[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WithoutPath.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WithoutPath.App_Start.NinjectWebCommon), "Stop")]

namespace WithoutPath.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Microsoft.AspNet.Identity;
    using WithoutPath.DAL;
    using WithoutPath.BL;
    using WithoutPath.Global.Config;
    using WithoutPath.Mappers;
    using WithoutPath.EVEAPI;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {          
            kernel.Bind<IRepository>().To<SqlRepository>().InRequestScope();
            kernel.Bind<IUserStore<User>>().To<DAL.Identity.ApplicationUserStore<User>>().InRequestScope();
            kernel.Bind<IConfig>().To<Config>().InSingletonScope();
            kernel.Bind<IMapper>().To<CommonMapper>().InSingletonScope();
            kernel.Bind<IEVEProvider>().To<EVEProvider>().InSingletonScope();
            kernel.Bind<ILogic>().To<WithoutLogic>().InRequestScope();
        }        
    }
}
