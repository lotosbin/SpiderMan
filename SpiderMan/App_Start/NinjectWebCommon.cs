[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SpiderMan.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SpiderMan.App_Start.NinjectWebCommon), "Stop")]

namespace SpiderMan.App_Start {
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using sharp_net.Mongo;
    using SpiderMan.Entity;
    using System.Web.Http;
    using System.Configuration;
    using Baozou.Entity;

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
            try {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                //Ninject.Web.WebApi don't support mvc4 now. fix issus form http://stackoverflow.com/a/10855037/346701
                GlobalConfiguration.Configuration.DependencyResolver = new SpiderMan.App_Start.NinjectDependencyResolver(kernel);

                Kernel = kernel;
                return kernel;
            } catch {
                kernel.Dispose();
                throw;
            }
        }

        public static IKernel Kernel { get; private set; }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            //Todo: 为什么没有bind的情况下controller构造函数依然能得到对象实体?
            kernel.Bind<IMongoRepo<GgpttCard>>().To<MongoRepo<GgpttCard>>();
            kernel.Bind<IMongoRepo<AdianboVideo>>().To<MongoRepo<AdianboVideo>>();
            kernel.Bind<IMongoRepo<Shudong>>().To<MongoRepo<Shudong>>();
            kernel.Bind<IMongoRepo<TaskModel>>().To<MongoRepo<TaskModel>>();
            kernel.Bind<IMongoRepo<Site>>().To<MongoRepo<Site>>();
            kernel.Bind<IMongoRepo<Avator>>().To<MongoRepo<Avator>>();
            kernel.Bind<IMongoRepo<UserName>>().To<MongoRepo<UserName>>();

            string BaozouConStr = ConfigurationManager.ConnectionStrings["BaozouMongoServer"].ConnectionString;
            kernel.Bind<IMongoRepo<Team>>().To<MongoRepo<Team>>().WithConstructorArgument("conStr", BaozouConStr);
            kernel.Bind<IMongoRepo<LiveVideo>>().To<MongoRepo<LiveVideo>>().WithConstructorArgument("conStr", BaozouConStr);
            kernel.Bind<IMongoRepo<Match>>().To<MongoRepo<Match>>().WithConstructorArgument("conStr", BaozouConStr);
        }
    }
}
