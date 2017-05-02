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
        public IQueryable<Role> Roles
        {
            get
            {
                return Db.Roles;
            }
        }

        public IResult CreateRole(Role instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Roles.Add(instance);
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

        public IResult UpdateRole(Role instance)
        {
            try
            {
                var cache = Db.Roles.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.Name = instance.Name;
                    cache.Code = instance.Code;
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Role with ID {0} not found", instance.Id)
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

        public IResult RemoveRole(int idRole)
        {
            try
            {
                var instance = Db.Roles.FirstOrDefault(p => p.Id == idRole);
                if (instance != null)
                {
                    Db.Roles.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Role with ID {0} not found", idRole)
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
