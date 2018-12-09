using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tmpps.Infrastructure.Autofac;
using Tmpps.Infrastructure.Autofac.Configuration;
using Tmpps.Infrastructure.Common.DependencyInjection.Builder.Interfaces;
using Tmpps.Infrastructure.Common.Foundation;
using Tmpps.Infrastructure.Common.Foundation.Interfaces;
using Tmpps.Infrastructure.SQS;
using Tmpps.Infrastructure.SQS.Interfaces;
using UseCases;
using UseCases.Interfaces;

namespace Deploys.SQS.Configuration
{
    public class DIModule : IDIModule
    {
        private Assembly executeAssembly;
        private string rootPath;
        private IConfigurationRoot configurationRoot;
        private ILoggerFactory loggerFactory;
        private CommonDIModule commonAutofacModule;

        public DIModule(Assembly executeAssembly, string rootPath, IConfigurationRoot configurationRoot, ILoggerFactory loggerFactory)
        {
            this.executeAssembly = executeAssembly;
            this.rootPath = rootPath;
            this.configurationRoot = configurationRoot;
            this.loggerFactory = loggerFactory;
            this.commonAutofacModule = new CommonDIModule(executeAssembly, rootPath, loggerFactory);
        }

        public void DefineModule(IDIBuilder builder)
        {
            var mapRegister = new MapRegister();
            builder.RegisterInstance(mapRegister, x => x.As<IMapRegister>());
            builder.RegisterModule(this.commonAutofacModule);
            builder.RegisterModule(new AutofacDIModule());
            builder.RegisterModule(new SQSDIModule());
            builder.RegisterModule(new UseCasesDIModule());
            builder.RegisterInstance(this.configurationRoot, x => x.As<IConfigurationRoot>());
            builder.RegisterType<Config>(x =>
                x.As<ISQSConfig>()
                .As<ISQSDeploysConfig>()
                .SingleInstance());
        }
    }
}