using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UseCases.Interfaces;

namespace UseCases
{
    public class QueueDeleteUseCase : IQueueDeleteUseCase
    {
        private ISQSDeploysConfig config;
        private ILogger logger;
        private IQueueDeployService queueDeployService;

        public QueueDeleteUseCase(
            ISQSDeploysConfig config,
            IQueueDeployService queueDeployService,
            ILogger logger)
        {
            this.config = config;
            this.queueDeployService = queueDeployService;
            this.logger = logger;
        }

        public async Task<int> DeleteAsync()
        {
            var tasks = this.config.DeleteQueues.Select(async queue =>
            {
                try
                {
                    await this.queueDeployService.DeleteQueueAsync(queue);
                    this.logger.LogInformation($"Complete delete {queue}");
                    return true;
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, $"Error delete {queue}");
                    return false;
                }
            });
            return (await Task.WhenAll(tasks)).Count(x => !x);
        }
    }
}