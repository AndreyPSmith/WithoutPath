using System;
using System.Linq;
using System.Web.Mvc;
using WithoutPath.DAL;
using System.Text;
using System.Collections.Generic;
using WithoutPath.Attribute;

namespace WithoutPath.Areas.Admin.Controllers
{
    public class UserController : AdminController
    {
        public ActionResult Index(int id = 0)
        {
            ViewBag.Error = TempData["Error"];

            List<Character> characters = null;
            if(User.IsInRole("admin"))
            {
                var UserId = id == 0 ? CurrentUser.Id : id;
                characters = Repository.Characters.Where(x => x.UserID == UserId && !x.IsDeleted).ToList();
            }
            else
            {
                characters = Repository.Characters.Where(x => x.UserID == CurrentUser.Id && !x.IsDeleted).ToList();
            }

            return View(characters);
        }

        public ActionResult Authorize()
        {
            return Redirect(EVEProvider.Authorize(HostName +"/Admin/User/AddCharacter"));
        }

        public ActionResult AddCharacter()
        {
            TempData["Error"] = null;
            try
            {
                if (Request.Params.AllKeys.Contains("code"))
                {
                    var code = Request.Params["code"];

                    var authResponse = EVEProvider.GetAccessToken(code);
                    if (authResponse != null)
                    {
                        var character = EVEProvider.VerifyCharacter(authResponse.access_token);
                        if (Repository.Characters.Any(x => !x.IsDeleted && x.Name == character.CharacterName))
                        {
                            TempData["Error"] = "Персонаж уже зарегестрирован!";
                            return RedirectToAction("Index", "User", new { area = "admin" });
                        }

                        var characterExtend = EVEProvider.GetCharacter(character.CharacterID);
                        var corp = Repository.Corporations.FirstOrDefault(x => x.EveID == characterExtend.corporation_id);
                        if (corp == null)
                        {
                            TempData["Error"] = "Для Вашей корпорации нет разрешения на доступ!";
                            return RedirectToAction("Index", "User", new { area = "admin" });
                        }

                        var token = new Token
                        {
                            AccessToken = authResponse.access_token,
                            TokenType = authResponse.token_type,
                            ExpiresIn = authResponse.expires_in,
                            RefreshToken = authResponse.refresh_token,
                            Expires = DateTime.Now.AddSeconds(authResponse.expires_in - 1)
                        };

                        Repository.CreateToken(token);
                        var result = Repository.CreateCharacter(new Character
                        {
                            Name = character.CharacterName,
                            EveID = character.CharacterID,
                            UserID = CurrentUser.Id,
                            TokenID = token.Id,
                            CorporationID = corp.Id,
                            IsMain = true,
                            Ship = null,
                            IsOnline = false
                        });

                        if (result.IsError)
                            TempData["Error"] = result.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index", "User", new { area = "admin" });
        }

        [HttpPost]
        public ActionResult DeleteCharacter(int Id)
        {
            TempData["Error"] = null;

            if (CurrentUser.Banned.HasValue && CurrentUser.Banned.Value)
            {
                TempData["Error"] = "Character not found!";
                return RedirectToAction("Index", "User", new { area = "admin" });
            }

            var character = Repository.Characters.FirstOrDefault(x => x.Id == Id && x.UserID == CurrentUser.Id && !x.IsDeleted);
            if (character != null)
            {
                var result = Repository.RemoveCharacter(Id);
                if (result.IsError)
                    ViewBag.Error = result.Message;
            }
            else
            {
                ViewBag.Error = "Character not found!";
            }
            return RedirectToAction("Index", "User", new { area = "admin" });
        }

        [HttpPost]
        public ActionResult SetMain(int mainId)
        {
            TempData["Error"] = null;
            var character = Repository.Characters.FirstOrDefault(x => x.Id == mainId && x.UserID == CurrentUser.Id && !x.IsDeleted);
            if (character != null)
            {
                character.IsMain = true;
                var result = Repository.SetMainCharacter(character);
                if (result.IsError)
                    ViewBag.Error = result.Message;
            }
            else
            {
                ViewBag.Error = "Character not found!";
            }

            return RedirectToAction("Index", "User", new { area = "admin" });
        }
    }
}