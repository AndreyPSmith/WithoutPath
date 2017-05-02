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
        public IQueryable<SystemType> SystemTypes
        {
            get
            {
                return Db.SystemTypes;
            }
        }

        public IResult CreateSystemType(SystemType instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.SystemTypes.Add(instance);
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

        public IResult RemoveSystemType(int idSystemType)
        {
            try
            {
                var instance = Db.SystemTypes.FirstOrDefault(p => p.Id == idSystemType);
                if (instance != null)
                {
                    Db.SystemTypes.Remove(instance);
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("SystemType with ID {0} not found", idSystemType)
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