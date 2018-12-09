using System.Collections.Generic;
using Tmpps.Infrastructure.SQS.Models;

namespace UseCases.Interfaces
{
    public interface ISQSDeploysConfig
    {
        IEnumerable<SQSQueue> Queues { get; }
        IEnumerable<string> DeleteQueues { get; }
    }
}