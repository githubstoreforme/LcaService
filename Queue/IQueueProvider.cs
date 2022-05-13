using Azure.Storage.Queues.Models;
using System.Threading.Tasks;

namespace LcaService.Queue
{
    public interface IQueueProvider
    {
        Task<int> Count();
        Task<QueueMessage> GetMessage();
        Task Delete(QueueMessage message);
        Task<SendReceipt> SendMessage(string message);
    }
}