using LcaService.DataContracts;
using System.Threading.Tasks;

namespace LcaService.OrderStorage
{
    public interface IOrderStorageService
    {
        Task AddEntity(Order entity);
        Task UpdateEntity(Order entity);
        Task DeleteAsync(Order entity);
        Task<Order> RetrieveAsync(string id);
    }
}