using LcaService.DataContracts;
using LcaService.LetsEncrypt;
using LcaService.OrderStorage;
using LcaService.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LcaService.Worker
{
    public class PlaceOrderWorkflowManager : BackgroundService
    {
        private readonly ILogger<PlaceOrderWorkflowManager> _logger;
        private readonly IAcmeProvider _acmeProvider;
        private readonly IQueueProvider _inprogressQueueProvider;
        private readonly IQueueProvider _tobeStartedQueueProvider;
        private readonly IOrderStorageService _orderStorageService;


        public PlaceOrderWorkflowManager(ILogger<PlaceOrderWorkflowManager> logger, IAcmeProvider acmeProvider, IOrderStorageService orderStorageService)
        {
            _logger = logger;
            _acmeProvider = acmeProvider;
            _inprogressQueueProvider = new QueueProvider("inprogress");
            _tobeStartedQueueProvider = new QueueProvider("tobestarted");
            _orderStorageService = orderStorageService;
        }

        protected  async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await  Dowork(stoppingToken);
        }

        private async Task Dowork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Reading from queue");

                    if (await _tobeStartedQueueProvider.Count() != 0 && await _inprogressQueueProvider.Count() < 5)
                    {
                        var queueMessage = await _tobeStartedQueueProvider.GetMessage();
                        if (queueMessage != null && queueMessage.MessageText != null)
                        {
                            var order = JsonSerializer.Deserialize<Order>(queueMessage.MessageText);

                            var uri = await _acmeProvider.CreateOrder(new[] { order.SAN });

                            order.Status = "InProgress";
                            order.link = uri;
                            await _orderStorageService.UpdateEntity(order);
                            await _inprogressQueueProvider.SendMessage(queueMessage.MessageText);
                            await _tobeStartedQueueProvider.Delete(queueMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                }
            }
        }
    }
}
