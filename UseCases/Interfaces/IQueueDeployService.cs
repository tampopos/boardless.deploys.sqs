using System.Threading.Tasks;
using UseCases.Models;

namespace UseCases.Interfaces
{
    public interface IQueueDeployService
    {
        Task<string> GetQueueUrlAsync(string queueName);
        Task<string> CreateQueueAsync(SQSQueue queue);
        Task SetQueueAttributesAsync(string queueUrl, SQSQueue queue);
        Task<string> GetQueueArnAsync(string queueName);
        Task SetDeadLetterSettingsAsync(string queueUrl, SQSQueue queue);
        Task DeleteQueueAsync(string queueName);
    }
}