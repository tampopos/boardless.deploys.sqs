using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tmpps.Infrastructure.Common.DependencyInjection.Interfaces;
using Tmpps.Infrastructure.Common.Foundation.Exceptions;
using Tmpps.Infrastructure.Common.IO.Interfaces;
using Tmpps.Infrastructure.SQS.Interfaces;
using Tmpps.Infrastructure.SQS.Models;
using UseCases.Interfaces;

namespace UseCases
{
    public class QueueDeployUseCase : IQueueDeployUseCase
    {
        private ISQSDeploysConfig config;
        private IScopeProvider scopeProvider;
        private ILogger logger;

        public QueueDeployUseCase(
            ISQSDeploysConfig config,
            IScopeProvider scopeProvider,
            ILogger logger)
        {
            this.config = config;
            this.scopeProvider = scopeProvider;
            this.logger = logger;
        }

        public async Task<int> DeployAsync()
        {
            var queues = this.config.Queues.ToArray();
            for (int i = 0; i < 5; i++)
            {
                queues = (await this.DeployAsync(queues)).ToArray();
                if (queues.Length == 0)
                {
                    return 0;
                }
            }
            if (queues.Length > 0)
            {
                this.logger.LogError($"Queueの展開に失敗しました。({string.Join(",",queues.Select(q=>q.Name))})");
                return 1;
            }
            return 0;
        }
        public async Task<IEnumerable<SQSQueue>> DeployAsync(IEnumerable<SQSQueue> queues)
        {
            using(var semaphore = new SemaphoreSlim(4))
            {
                var tasks = queues.Select(async queue =>
                {
                    await semaphore.WaitAsync();
                    var result = await this.DeployAsync(queue);
                    semaphore.Release();
                    return result ? null : queue;
                });
                return (await Task.WhenAll(tasks)).Where(x => x != null);
            }
        }
        private async Task<bool> DeployAsync(SQSQueue queue, int count = 0)
        {
            try
            {
                using(var scope = this.scopeProvider.BeginLifetimeScope())
                {
                    var factory = scope.Resolve<ISQSQueueFactory>();
                    var queueUrl = await factory.GetQueueUrlAsync(queue.Name);
                    if (string.IsNullOrEmpty(queueUrl))
                    {
                        queueUrl = await factory.CreateQueueAsync(queue);
                    }
                    else
                    {
                        await factory.SetQueueAttributesAsync(queueUrl, queue);
                    }
                    await factory.SetDeadLetterSettingsAsync(queueUrl, queue);
                }
                this.logger.LogInformation($"Complete deploy {queue.Name}");
                return true;
            }
            catch (Exception ex)
            {
                if (count < 5)
                {
                    var next = count + 1;
                    var wait = next * 10;
                    this.logger.LogInformation($"Retry deploy {queue.Name}. wait {wait} sec.");
                    await Task.Delay(wait * 1000);
                    return await this.DeployAsync(queue, next);
                }
                this.logger.LogWarning(ex, $"Error deploy {queue.Name}");
                return false;
            }
        }
    }
}