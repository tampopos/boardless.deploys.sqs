using System.Collections.Generic;
using UseCases.Models;

namespace UseCases.Interfaces
{
    public interface ISQSDeploysConfig
    {
        IEnumerable<SQSQueue> Queues { get; }
        IEnumerable<string> DeleteQueues { get; }
    }
}