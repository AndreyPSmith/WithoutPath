using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public partial class SqlRepository
    {
        public IQueryable<Character> Characters
        {
            get
            {
                return Db.Characters;
            }
        }

        public IResult CreateCharacter(Character instance, bool IsSave = true)
        {
            try
            {
                if (instance.Id == 0)
                {
                    var main =  Db.Characters.FirstOrDefault(x => x.UserID == instance.UserID && x.IsMain.Value && !x.IsDeleted);
                    instance.IsMain = main == null;
                    instance.IsDeleted = false;

                    Db.Characters.Add(instance);
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

        public IResult SetMainCharacter(Character instance)
        {
            try
            {
                var cache = Db.Characters.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    Db.Characters.ToList().ForEach(x => x.IsMain = x.Id == instance.Id);

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Character with ID {0} not found", instance.Id)
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

        public IResult UpdateCharacter(Character instance, bool IsSave = true)
        {
            try
            {
                var cache = Db.Characters.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.CorporationID = instance.CorporationID;
                    cache.Ship = instance.Ship;
                    cache.SystemID = instance.SystemID;
                    cache.ShipTypeID = instance.ShipTypeID;
                    cache.IsOnline = instance.IsOnline;

                    if (IsSave) Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Character with ID {0} not found", instance.Id)
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

        public IResult RemoveCharacter(int idCharacter, bool IsSave = true)
        {
            try
            {
                var instance = Db.Characters.FirstOrDefault(p => p.Id == idCharacter);
                if (instance != null)
                {
                    if(instance.IsMain.HasValue && instance.IsMain.Value)
                    {
                        var newmain = Db.Characters.FirstOrDefault(p => p.Id != idCharacter);
                        if(newmain != null)
                            newmain.IsMain = true;
                    }

                    instance.IsDeleted = true;

                    if (IsSave) if (IsSave) Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Character with ID {0} not found", idCharacter)
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

