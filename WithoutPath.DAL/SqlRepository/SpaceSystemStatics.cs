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
        public IQueryable<SpaceSystemStatic> SpaceSystemStatics
        {
            get
            {
                return Db.SpaceSystemStatics;
            }
        }

        public IResult CreateSpaceSystemStatic(SpaceSystemStatic instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.SpaceSystemStatics.Add(instance);
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

        public IResult RemoveSpaceSystemStatic(int idSpaceSystemStatic)
        {
            try
            {
                var instance = Db.SpaceSystemStatics.FirstOrDefault(p => p.Id == idSpaceSystemStatic);
                if (instance != null)
                {
                    Db.SpaceSystemStatics.Remove(instance);
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("SpaceSystemStatic with ID {0} not found", idSpaceSystemStatic)
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