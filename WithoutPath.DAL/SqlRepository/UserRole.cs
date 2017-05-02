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
        public IQueryable<UserRole> UserRoles
        {
            get
            {
                return Db.UserRoles;
            }
        }

        public IResult CreateUserRole(UserRole instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.UserRoles.Add(instance);
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

        public IResult RemoveUserRole(int idUserRole)
        {
            try
            {
                var instance = Db.UserRoles.FirstOrDefault(p => p.Id == idUserRole);
                if (instance != null)
                {
                    Db.UserRoles.Remove(instance);
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("UserRole with ID {0} not found", idUserRole)
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
