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
        public IQueryable<Static> Statics
        {
            get
            {
                return Db.Statics;
            }
        }

        public IResult CreateStatic(Static instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Statics.Add(instance);
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

        public IResult RemoveStatic(int idStatic)
        {
            try
            {
                var instance = Db.Statics.FirstOrDefault(p => p.Id == idStatic);
                if (instance != null)
                {
                    Db.Statics.Remove(instance);
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Static with ID {0} not found", idStatic)
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