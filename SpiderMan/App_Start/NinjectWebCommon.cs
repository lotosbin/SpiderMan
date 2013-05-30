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
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            kernel.Bind<ArticleRespository>().To<ArticleRespository>();
            kernel.Bind<TaskRespository>().To<TaskRespository>();
            kernel.Bind<MongoRepository<Site>>().To<MongoRepository<Site>>();
            kernel.Bind<MongoRepository<Comment>>().To<MongoRepository<Comment>>();
            kernel.Bind<MongoRepository<TaskModel>>().To<MongoRepository<TaskModel>>();
            kernel.Bind<MongoRepository<User>>().To<MongoRepository<User>>();
        }
    }
}
