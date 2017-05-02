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
        public IQueryable<SpaceSystem> SpaceSystems
        {
            get
            {
                return Db.SpaceSystems;
            }
        }

        public IResult CreateSpaceSystem(SpaceSystem instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.SpaceSystems.Add(instance);
                    Db.SaveChanges();

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

        public IResult UpdateSpaceSystem(SpaceSystem instance)
        {
            try
            {
                var cache = Db.SpaceSystems.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.Note = instance.Note;
                    cache.Name = instance.Name;
                    cache.EveID = instance.EveID;
                    cache.IsWormhole = instance.IsWormhole;
                    cache.Security = instance.Security;
                    cache.TypeID = instance.TypeID;
                    cache.Warning = instance.Warning;

                    if (instance.IsHome)
                    {
                        var home = Db.SpaceSystems.FirstOrDefault(p => p.IsHome && p.Id != cache.Id);
                        if (home != null)
                            home.IsHome = false;

                        cache.IsHome = instance.IsHome;
                    }

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("SpaceSystem with ID {0} not found", instance.Id)
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

        public IResult RemoveSpaceSystemLinks(int id)
        {
            try
            {
                var cache = Db.SpaceSystems.FirstOrDefault(p => p.Id == id);
                if (cache != null)
                {
                    Db.Links.RemoveRange(cache.Links1);
                    Db.Links.RemoveRange(cache.Links);

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("SpaceSystem with ID {0} not found", id)
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

        public IResult RemoveSpaceSystem(int idSpaceSystem)
        {
            try
            {
                var instance = Db.SpaceSystems.FirstOrDefault(p => p.Id == idSpaceSystem);
                if (instance != null)
                {
                    Db.SpaceSystems.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("SpaceSystem with ID {0} not found", idSpaceSystem)
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
