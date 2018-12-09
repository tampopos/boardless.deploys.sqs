using Tmpps.Infrastructure.Common.DependencyInjection.Builder.Interfaces;
using Tmpps.Infrastructure.SQS;
using UseCases.Interfaces;

namespace UseCases
{
    public class UseCasesDIModule : IDIModule
    {
        public void DefineModule(IDIBuilder builder)
        {
            builder.RegisterType<QueueDeployUseCase>(x => x.As<IQueueDeployUseCase>());
            builder.RegisterType<QueueDeleteUseCase>(x => x.As<IQueueDeleteUseCase>());
            builder.RegisterModule(new SQSDeployDIModule());
        }
    }
}