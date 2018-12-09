using System.Threading.Tasks;

namespace UseCases.Interfaces
{
    public interface IQueueDeleteUseCase
    {
        Task<int> DeleteAsync();
    }
}