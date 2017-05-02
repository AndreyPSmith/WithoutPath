using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DAL;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Ninject;
using WithoutPath.DTO;

namespace WithoutPath.DAL.Identity
{
    public class ApplicationUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser> where TUser : User
    {
        [Inject]
        public IRepository Repository { get; set; }

        public ApplicationUserStore()
        {
        }

        public Task CreateAsync(TUser user)
        {
            return Task.Factory.StartNew(() => {
                if (user != null)
                    Repository.CreateUser(user);
            });
        }
        public Task<TUser> FindByIdAsync(String id)
        {
            return Task<TUser>.Factory.StartNew(() => {

                int ID = 0;
                if(int.TryParse(id,out ID))
                    return Repository.GetUser(ID) as TUser;

                return null;
            });
        }
        public Task<TUser> FindByNameAsync(String userName)
        {
            return Task<TUser>.Factory.StartNew(() => {
                return Repository.GetUser(userName) as TUser;
            });

        }
        public Task UpdateAsync(TUser user)
        {
            return Task.Factory.StartNew(() => {
                if (user != null)
                    Repository.UpdateUser(user);
            });
        }
        public Task DeleteAsync(TUser user)
        {
            return Task.Factory.StartNew(() => {
                if (user != null)
                    Repository.RemoveUser(user.Id);
            });

        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public Task SetPasswordHashAsync(TUser user, String passwordHash)
        {
            return Task.Factory.StartNew(() => {
                user.Password = passwordHash;
            });
        }
        public Task<String> GetPasswordHashAsync(TUser user)
        {
            return Task<string>.Factory.StartNew(() => {
                return user.Password;
            });
        }
        public Task<Boolean> HasPasswordAsync(TUser user)
        {
            return Task<Boolean>.Factory.StartNew(() => {
                return String.IsNullOrEmpty(user.Password);
            });
        }
    }
}
