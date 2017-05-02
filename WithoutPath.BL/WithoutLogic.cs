using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DAL;
using WithoutPath.DTO;
using Ninject;
using WithoutPath.EVEAPI;

namespace WithoutPath.BL
{
    public class WithoutLogic : ILogic
    {

        [Inject]
        public IRepository Repository { get; set; }

        [Inject]
        public IEVEProvider Provider { get; set; }

        public WithoutLogic() { }

        public MapModel GetMap()
        {
            Repository.UdateDBContext();
            var model = new MapModel();

            model.Systems = Repository.SpaceSystems.Where(x => x.IsHome ||
                                                     (x.IsWormhole && x.Characters.Any(y => !y.IsDeleted && y.IsOnline.HasValue && y.IsOnline.Value)) ||
                                                      x.Links.Any() || x.Links1.Any()).ToList()
                    .Select(x => new SystemsModel
                    {
                        Id = x.Id,
                        EveID = x.EveID,
                        Name = x.Name,
                        Note = x.Note,
                        Security = x.Security,
                        IsHome = x.IsHome,
                        IsWormhole = x.IsWormhole,
                        Warning = x.Warning.HasValue && x.Warning.Value,
                        SystemType = x.TypeID.HasValue ? new SystemTypeModel
                        {
                            Id = x.SystemType.Id,
                            Class = x.SystemType.Class,
                            Description = x.SystemType.Description,
                            Type = x.SystemType.Type
                        } : null,
                        Characters = x.Characters.Where(y => !y.IsDeleted && y.IsOnline == true).Select(y => new CharacterModel
                        {
                            EveID = y.EveID,
                            Name = y.Name,
                            Ship = y.Ship != null ? (y.ShipTypeID.HasValue ? y.ShipType.Name + " - " : "") + y.Ship : null
                        }).ToList(),
                        Statics = x.SpaceSystemStatics.Select(y => new StaticModel
                        {
                            Name = y.Static.Name,
                            Description = y.Static.Description
                        }).ToList()
                    }).ToList();

            model.Links = Repository.Links.Select(x => new LinksModel
            {
                Id = x.Id,
                From = x.SpaceSystem.Name,
                To = x.SpaceSystem1.Name,
                Time = x.Time,
                Status = x.Status
            }).ToList();

            return model;
        }

        // Изменить статус ссылки
        public IResult UpdateSystemStatus(int Id, string note, bool warning)
        {
            try
            {
                var system = Repository.SpaceSystems.FirstOrDefault(x => x.Id == Id);
                if (system != null)
                {
                    system.Note = note;
                    system.Warning = warning;
                    return Repository.UpdateSpaceSystem(system);
                }

                return new SimpleResult { IsError = true, Message = "Unable to update System!" };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public IResult RemoveSystem(int systemId)
        {
            try
            {
                return Repository.RemoveSpaceSystemLinks(systemId);
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        // Удалить ссылку
        public IResult RemoveLink(string system_1, string system_2)
        {
            try
            {
                var link = Repository.Links.FirstOrDefault(x => (x.SpaceSystem.Name == system_1 && x.SpaceSystem1.Name == system_2) ||
                                                                (x.SpaceSystem1.Name == system_1 && x.SpaceSystem.Name == system_2));

                if (link != null)
                    return Repository.RemoveLink(link.Id);

                return new SimpleResult { IsError = true, Message = "Link not found!" };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        // Добавить ссылку
        public IResult AddLink(string from, string to, int userId)
        {
            try
            {
                var character = Repository.Characters.FirstOrDefault(x => !x.IsDeleted && x.UserID == userId && x.IsMain.HasValue && x.IsMain.Value);
                var from_system = Repository.SpaceSystems.FirstOrDefault(x => x.Name == from);
                var to_system = Repository.SpaceSystems.FirstOrDefault(x => x.Name == to);
                if (character != null && from_system != null && to_system != null)
                {
                    return Repository.CreateLink(new Link
                    {
                        CharacterID = character.Id,
                        FromID = from_system.Id,
                        ToID = to_system.Id,
                        Time = DateTime.Now
                    });
                }

                return new SimpleResult { IsError = true, Message = "Unable to create Link!" };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        // Изменить статус ссылки
        public IResult UpdateLinkStatus(int Id, int status)
        {
            try
            {
                var link = Repository.Links.FirstOrDefault(x => x.Id == Id);
                if (link != null)
                {
                    link.Status = status;
                    return Repository.UpdateLink(link);
                }

                return new SimpleResult { IsError = true, Message = "Unable to update Link!" };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public User GetUser(string email)
        {
            return Repository.GetUser(email);
        }

        public string GetMainCharacterAccessToken(int userId)
        {
            string accessToken = null;

            var character = Repository.Characters.FirstOrDefault(x => !x.IsDeleted && (x.IsMain.HasValue && x.IsMain.Value) && x.UserID == userId);
            if (character != null)
            {
                var token = Repository.Tokens.FirstOrDefault(x => x.Id == character.TokenID);
                if (DateTime.Compare(token.Expires, DateTime.Now) < 0)
                {
                    var newToken = Provider.GetAccessToken(token.RefreshToken, true);
                    token.AccessToken = newToken.access_token;
                    token.TokenType = newToken.token_type;
                    token.ExpiresIn = newToken.expires_in;
                    token.RefreshToken = newToken.refresh_token;
                    token.Expires = DateTime.Now.AddSeconds(newToken.expires_in - 1);
                    Repository.UpdateToken(token);
                }

                accessToken = token.AccessToken;
            }

            return accessToken;
        }

        public IResult UpdateCharactersLocation()
        {
            try
            {
                // удаляем ссылки старше 24х часов
                Repository.RemoveOldLinks();

                // обновление токенов
                foreach (var token in Repository.Tokens.Where(x => DateTime.Compare(x.Expires, DateTime.Now) < 0))
                {
                    var newToken = Provider.GetAccessToken(token.RefreshToken, true);
                    token.AccessToken = newToken.access_token;
                    token.TokenType = newToken.token_type;
                    token.ExpiresIn = newToken.expires_in;
                    token.RefreshToken = newToken.refresh_token;
                    token.Expires = DateTime.Now.AddSeconds(newToken.expires_in - 1);
                    Repository.UpdateToken(token, false);
                }

                Repository.SaveChanges();

                var characterCache = new List<Character>();

                foreach (var character in Repository.Characters.Where(x => !x.IsDeleted))
                {
                    // токен доступа
                    var token = Repository.Tokens.FirstOrDefault(x => x.Id == character.TokenID);
                    if (token != null && DateTime.Compare(token.Expires, DateTime.Now) > 0)
                    {
                        var cache = new Character
                        {
                            Id = character.Id,
                            EveID = character.EveID,
                            CorporationID = character.CorporationID,
                            Ship = character.Ship,
                            SystemID = character.SystemID,
                            ShipTypeID = character.ShipTypeID,
                            IsOnline = character.IsOnline,
                            SpaceSystem = character.SpaceSystem
                        };

                        // местоположение персонажа
                        var location = Provider.GetCharacterLocationCrest(cache.EveID, token.AccessToken);
                        if (location.solar_system != null)
                        {
                            cache.IsOnline = true;

                            // информация о системе
                            var system = Provider.GetSolarSystem(location.solar_system.solar_system_id);
                            if (system != null)
                            {
                                var spaceSystem = Repository.SpaceSystems.FirstOrDefault(x => x.EveID == system.system_id || x.Name == system.name);
                                if (spaceSystem == null)
                                {
                                    spaceSystem = new SpaceSystem
                                    {
                                        EveID = system.system_id,
                                        IsWormhole = system.security_class == null,
                                        Name = system.name,
                                        Security = system.security_class == null ? null : (double?)Math.Round(system.security_status.Value, MidpointRounding.AwayFromZero),
                                        IsHome = false,
                                    };
                                    Repository.CreateSpaceSystem(spaceSystem);
                                }
                                else if (spaceSystem.EveID == 0)
                                {
                                    spaceSystem.EveID = system.system_id;
                                    Repository.UpdateSpaceSystem(spaceSystem);
                                }

                                if (cache.SystemID.HasValue &&
                                    cache.SystemID.Value != spaceSystem.Id &&
                                   (cache.SpaceSystem.IsWormhole || spaceSystem.IsWormhole))
                                {
                                    bool IsPodExpress = false;
                                    if (!spaceSystem.IsWormhole)
                                    {
                                        // esi api возвращает еще и id структуры, если персонаж был в спейсе ВХ а потом
                                        // сразу в импе на станке, то очевидно это под экспресс
                                        var esi_location = Provider.GetCharacterLocation(cache.EveID, token.AccessToken);
                                        IsPodExpress = esi_location.structure_id.HasValue;
                                    }

                                    if (!IsPodExpress)
                                    {
                                        var link = Repository.Links.FirstOrDefault(x =>
                                            (x.FromID == cache.SpaceSystem.Id && x.ToID == spaceSystem.Id) ||
                                            (x.ToID == cache.SpaceSystem.Id && x.FromID == spaceSystem.Id));

                                        if (link == null)
                                        {
                                            Repository.CreateLink(new Link
                                            {
                                                CharacterID = cache.Id,
                                                Status = 0,
                                                Time = DateTime.Now,
                                                SpaceSystem = cache.SpaceSystem, // From
                                                SpaceSystem1 = spaceSystem // To
                                            });
                                        }
                                    }
                                }

                                cache.SystemID = spaceSystem.Id;

                                // информация о корабле
                                var ship = Provider.GetCharacterShip(cache.EveID, token.AccessToken);
                                if (ship != null)
                                {
                                    cache.Ship = ship.ship_name;
                                    var type = Repository.ShipTypes.FirstOrDefault(x => x.EveID == ship.ship_type_id);
                                    if (type == null)
                                    {
                                        var typeResponse = Provider.GetShipType(ship.ship_type_id);
                                        if (typeResponse != null)
                                        {
                                            type = new ShipType
                                            {
                                                EveID = typeResponse.type_id,
                                                Name = typeResponse.name
                                            };
                                            Repository.CreateShipType(type);
                                        }
                                    }

                                    cache.ShipTypeID = type != null ? type.Id : 0;
                                }
                                else
                                {
                                    cache.Ship = null;
                                }
                            }
                        }
                        else
                        {
                            cache.IsOnline = false;
                        }

                        characterCache.Add(cache);
                    }
                }

                characterCache.ForEach(x => Repository.UpdateCharacter(x, false));
                Repository.SaveChanges();

                return new SimpleResult { IsError = false };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }
    }
}
