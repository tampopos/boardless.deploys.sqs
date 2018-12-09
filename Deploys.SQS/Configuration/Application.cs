using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Tmpps.Infrastructure.Common.IO.Interfaces;
using UseCases.Interfaces;

namespace Deploys.SQS.Configuration
{
    public class Application
    {
        private CommandLineApplication application;
        private ILogger logger;
        private IQueueDeployUseCase queueDeployUseCase;
        private IQueueDeleteUseCase queueDeleteUseCase;

        public Application(
            CommandLineApplication application,
            ILogger logger,
            IQueueDeployUseCase queueDeployUseCase,
            IQueueDeleteUseCase queueDeleteUseCase
        )
        {
            this.application = application;
            this.logger = logger;
            this.queueDeployUseCase = queueDeployUseCase;
            this.queueDeleteUseCase = queueDeleteUseCase;
        }

        public int Execute(string[] args)
        {
            this.application.Name = "Boardless.Deploys.SQS";
            this.application.Description = "Boardless Db の展開アプリ";
            this.application.HelpOption("-h|--help");
            this.application.OnExecute(async() =>
            {
                return await this.ExecuteOnErrorHandleAsync("deploy", async() =>
                {
                    return (await this.queueDeployUseCase.DeployAsync()) + (await this.queueDeleteUseCase.DeleteAsync());
                });
            });

            return this.application.Execute(args);
        }

        private async Task<int> ExecuteOnErrorHandleAsync(string commandName, Func<Task<int>> func)
        {
            try
            {
                this.logger?.LogInformation($"Start {commandName}");
                var res = await func();
                this.logger?.LogInformation($"End {commandName}");
                return res;
            }
            catch (OperationCanceledException ex)
            {
                this.logger?.LogWarning(ex.Message);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, $"Error {commandName}");
            }
            return 1;
        }
    }
}