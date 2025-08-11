using NHT_Marine_BE.Data.Dtos.Order;
using NHT_Marine_BE.Data.Queries;
using NHT_Marine_BE.Models.Transaction;

namespace NHT_Marine_BE.Interfaces.Repositories
{
    public interface IStatusTransitionRepository
    {
        Task<List<StatusTransitionGroupDto>> GetAllTransitions(BaseQueryObject queryObject);
        Task<StatusTransition?> GetTransitionById(int transitionId);
        Task<StatusTransition?> GetTransitionByFromAndToStatusId(int fromStatusId, int toStatusId);
        Task AddStatusTransition(StatusTransition statusTransition);
        Task UpdateStatusTransition(StatusTransition statusTransition);
        Task DeleteStatusTransition(StatusTransition statusTransition);
    }
}
