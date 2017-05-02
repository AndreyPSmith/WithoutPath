using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public partial class SqlRepository
    {
        public IQueryable<Token> Tokens
        {
            get
            {
                return Db.Tokens;
            }
        }

        public IResult CreateToken(Token instance, bool IsSave = true)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Tokens.Add(instance);
                    if (IsSave) Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = "id not equal 0"
                };
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

        public IResult UpdateToken(Token instance, bool IsSave = true)
        {
            try
            {
                var cache = Db.Tokens.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.AccessToken = instance.AccessToken;
                    cache.TokenType = instance.TokenType;
                    cache.ExpiresIn = instance.ExpiresIn;
                    cache.RefreshToken = instance.RefreshToken;
                    cache.Expires = instance.Expires;

                    if (IsSave) Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Token with ID {0} not found", instance.Id)
                };
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

        public IResult RemoveToken(int idToken, bool IsSave = true)
        {
            try
            {
                var instance = Db.Tokens.FirstOrDefault(p => p.Id == idToken);
                if (instance != null)
                {
                    Db.Tokens.Remove(instance);
                    if (IsSave) Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Token with ID {0} not found", idToken)
                };

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