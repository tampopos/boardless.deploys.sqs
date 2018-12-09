using System.Threading.Tasks;

namespace UseCases.Interfaces
{
    public interface IQueueDeployUseCase
    {
        Task<int> DeployAsync();
    }
}