using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tmpps.Infrastructure.SQS.Interfaces;
using UseCases.Interfaces;

namespace UseCases
{
    public class QueueDeleteUseCase : IQueueDeleteUseCase
    {
        private ISQSDeploysConfig config;
        private ILogger logger;
        private ISQSQueueFactory sqsQueueFactory;

        public QueueDeleteUseCase(
            ISQSDeploysConfig config,
            ISQSQueueFactory sqsQueueFactory,
            ILogger logger)
        {
            this.config = config;
            this.sqsQueueFactory = sqsQueueFactory;
            this.logger = logger;
        }

        public async Task<int> DeleteAsync()
        {
            var tasks = this.config.DeleteQueues.Select(async queue =>
            {
                try
                {
                    await this.sqsQueueFactory.DeleteQueueAsync(queue);
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