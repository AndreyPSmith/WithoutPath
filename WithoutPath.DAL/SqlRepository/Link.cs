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
        public IQueryable<Link> Links
        {
            get
            {
                return Db.Links;
            }
        }

        public IResult CreateLink(Link instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Links.Add(instance);
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

        public IResult UpdateLink(Link instance)
        {
            try
            {
                var cache = Db.Links.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.FromID = instance.FromID;
                    cache.ToID = instance.ToID;
                    cache.FromID = instance.FromID;
                    cache.Status = instance.Status;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Link with ID {0} not found", instance.Id)
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

        public IResult RemoveLink(int idLink)
        {
            try
            {
                var instance = Db.Links.FirstOrDefault(p => p.Id == idLink);
                if (instance != null)
                {
                    Db.Links.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Link with ID {0} not found", idLink)
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

        public IResult RemoveOldLinks()
        {
            try
            {
                Db.Links.RemoveRange(Db.Links.ToList().Where(x => DateTime.Compare(x.Time.AddDays(1), DateTime.Now) < 1));
                Db.SaveChanges();

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
