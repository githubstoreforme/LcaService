using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.Queue
{
    public class QueueProvider : IQueueProvider
    {
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=letsencryptssamacc;AccountKey=wQ686s+k+tnv0qdUB5H88RLxw3xdc8h156aFNx2K2gBh1roFQIxtPJa02qLeFfWp0mIzkF1pZAae+AStmzCgxQ==;EndpointSuffix=core.windows.net";
        private readonly QueueClient _queueClient;
        public QueueProvider(string queuename)
        {
            _queueClient = new QueueClient(_connectionString, queuename);
            _queueClient.CreateIfNotExists();
        }

        public async Task<int> Count()
        {
            var properties = await _queueClient.GetPropertiesAsync();
            return properties.Value.ApproximateMessagesCount;
        }

        public async Task<SendReceipt> SendMessage(string message)
        {
            return await _queueClient.SendMessageAsync(message);           

        }

        public async Task<QueueMessage> GetMessage()
        {
            return await _queueClient.ReceiveMessageAsync();            
        }

        public async Task Delete(QueueMessage message)
        {
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }
    }
}
