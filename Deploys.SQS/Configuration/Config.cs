using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Tmpps.Infrastructure.SQS;
using Tmpps.Infrastructure.SQS.Interfaces;
using UseCases.Interfaces;
using UseCases.Models;

namespace Deploys.SQS.Configuration
{
    public class Config : ISQSConfig, ISQSDeploysConfig
    {
        private IConfigurationRoot configurationRoot;
        public Config(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
            this.AwsAccessKeyId = this.configurationRoot.GetValue<string>(nameof(this.AwsAccessKeyId));
            this.AwsSecretAccessKey = this.configurationRoot.GetValue<string>(nameof(this.AwsSecretAccessKey));
            this.ServiceURL = this.configurationRoot.GetValue<string>(nameof(this.ServiceURL));
            this.SQSMessageSendSettings = this.configurationRoot.GetSQSMessageSendSettings(nameof(this.SQSMessageSendSettings));
            this.SQSMessageReceiveSettings = this.configurationRoot.GetSQSMessageReceiveSettings(nameof(this.SQSMessageReceiveSettings));
            this.MaxConcurrencyReceive = this.configurationRoot.GetValue<int>(nameof(this.MaxConcurrencyReceive));
            this.Queues = this.configurationRoot
                .GetSection(nameof(this.Queues))
                .GetChildren()
                .Select(x => new SQSQueue
                {
                    Name = x.GetValue<string>(nameof(SQSQueue.Name)),
                        DelaySeconds = x.GetValue<int>(nameof(SQSQueue.DelaySeconds), 0),
                        MaximumMessageSize = x.GetValue<int>(nameof(SQSQueue.MaximumMessageSize), 262144),
                        MessageRetentionPeriod = x.GetValue<int>(nameof(SQSQueue.MessageRetentionPeriod), 345600),
                        ReceiveMessageWaitTimeSeconds = x.GetValue<int>(nameof(SQSQueue.ReceiveMessageWaitTimeSeconds)),
                        VisibilityTimeout = x.GetValue<int>(nameof(SQSQueue.VisibilityTimeout), 600),
                        DeadLetterQueueName = x.GetValue<string>(nameof(SQSQueue.DeadLetterQueueName)),
                        MaxReceiveCount = x.GetValue<int>(nameof(SQSQueue.MaxReceiveCount), 10),
                        IsFifo = x.GetValue<bool>(nameof(SQSQueue.IsFifo), false),
                })
                .ToArray();
            this.DeleteQueues = this.configurationRoot
                .GetSection(nameof(this.DeleteQueues))
                .GetChildren()
                .Select(x => x.Get<string>())
                .ToArray();
        }

        public string AwsAccessKeyId { get; }
        public string AwsSecretAccessKey { get; }
        public string ServiceURL { get; }
        public IDictionary<string, SQSMessageSendSetting> SQSMessageSendSettings { get; }
        public IDictionary<string, SQSMessageReceiveSetting> SQSMessageReceiveSettings { get; }
        public int MaxConcurrencyReceive { get; }

        public IEnumerable<SQSQueue> Queues { get; }

        public IEnumerable<string> DeleteQueues { get; }
    }
}