using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WithoutPath.DAL;
using System.Web.Mvc;


namespace WithoutPath
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(DependencyResolver.Current.GetService<IUserStore<User>>());
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {

            return Task<IdentityResult>.Run(
                async () =>
                {
                    try
                    {
                        user.Password = password;
                        await Store.CreateAsync(user);
                    }
                    catch (Exception ex)
                    {
                        return new IdentityResult(ex.Message);
                    }

                    return IdentityResult.Success;
                });
        }

        public override Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            return Task.Run(() =>
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Email));
                user.UserRoles.ToList().ForEach(role =>
                {
                    if (role.Role != null)
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.Role.Code));
                });

                return identity;
            });
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<User, string>
    {

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public override Task SignInAsync(User user, bool isPersistent, bool rememberBrowser)
        {
            return Task.Run(
            async () =>
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                if (!(user.Banned.HasValue && user.Banned.Value))
                {
                    var Properties = new AuthenticationProperties();
                    Properties.IsPersistent = isPersistent;
                    if (!isPersistent)
                        Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30);
                    var identity = await user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
                    AuthenticationManager.SignIn(Properties, identity);
                }
            });
        }

        public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return Task<SignInStatus>.Run(
              async () =>
              {

                  if (UserManager == null)
                  {
                      return SignInStatus.Failure;
                  }

                  AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                  var user = await UserManager.FindByNameAsync(userName);
                  if (user != null && user.Password == password && !(user.Banned.HasValue && user.Banned.Value))
                  {
                      var Properties = new AuthenticationProperties();
                      Properties.IsPersistent = isPersistent;
                      if (!isPersistent)
                          Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30);
                      var identity = await user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
                      AuthenticationManager.SignIn(Properties, identity);

                      return SignInStatus.Success;
                  }

                  return SignInStatus.Failure;
              });
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
