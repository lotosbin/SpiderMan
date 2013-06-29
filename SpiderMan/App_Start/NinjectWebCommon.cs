[assembly: WebActivator.PreApplicationStartMethod(typeof(SpiderMan.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(SpiderMan.App_Start.NinjectWebCommon), "Stop")]

namespace SpiderMan.App_Start {
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using SpiderMan.Respository;
    using MongoRepository;
    using SpiderMan.Models;
    using System.Web.Http;

    public static class NinjectWebCommon {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop() {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel() {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);

            //Ninject.Web.WebApi don't support mvc4 now. fix issus form http://stackoverflow.com/a/10855037/346701
            GlobalConfiguration.Configuration.DependencyResolver = new SpiderMan.App_Start.NinjectDependencyResolver(kernel);

            Kernel = kernel;
            return kernel;
        }

        public static IKernel Kernel { get; private set; }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            kernel.Bind<Respositorys>().To<Respositorys>();
            kernel.Bind<HuanleRespository>().To<HuanleRespository>();
        }
    }
}
