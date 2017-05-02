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

        public IQueryable<User> Users
        {
            get
            {
                return Db.Users;
            }
        }

        public IResult CreateUser(User instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    instance.ActivatedLink = Guid.NewGuid().ToString("N");

                    if (instance.Password == null)
                        instance.Password = Guid.NewGuid().ToString("N").Substring(0, 6);

                    instance.AddedDate = DateTime.Now;
                    instance.UserRoles = instance.UserRoles.Select(x => new UserRole { User = instance, RoleId = x.RoleId }).ToList();

                    Db.Users.Add(instance);
                    Db.UserRoles.Add(new UserRole
                    {
                        User = instance,
                        RoleId = 1
                    });

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

        public User GetUser(string email)
        {
            return Db.Users.FirstOrDefault(p => p.Email == email);
        }

        public User GetUser(int idUser)
        {
            return Db.Users.FirstOrDefault(p => p.Id == idUser);
        }

        public User Login(string email, string password)
        {
            return Db.Users.FirstOrDefault(p => p.Email == email && p.Password == password);
        }

        public IResult UpdateUser(User instance)
        {
            try
            {
                var cache = Db.Users.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.Email = instance.Email;
                    cache.Note = instance.Note;
                    cache.Banned = instance.Banned;

                    if(cache.Banned.HasValue && cache.Banned.Value)
                        Db.Characters.Where(x => x.UserID == cache.Id).ToList().ForEach(x => { x.IsDeleted = true; });

                    Db.UserRoles.RemoveRange(Db.UserRoles.Where(x => x.UserId == instance.Id));
                    cache.UserRoles = instance.UserRoles.Select(x => new UserRole { UserId = x.UserId, RoleId = x.RoleId }).ToList();
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("User with ID {0} not found", instance.Id)
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

        public IResult ActivateUser(User instance)
        {
            try
            {
                var cache = Db.Users.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.ActivatedDate = DateTime.Now;
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("User with ID {0} not found", instance.Id)
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


        public IResult RemoveUser(int idUser)
        {
            try
            {
                var cache = Db.Users.FirstOrDefault(p => p.Id == idUser);
                if (cache != null)
                {
                    Db.Users.Remove(cache);
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("User with ID {0} not found", idUser)
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


        public IResult ChangePassword(User instance)
        {
            try
            {
                var cache = Db.Users.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.Password = instance.Password;
                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("User with ID {0} not found", instance.Id)
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