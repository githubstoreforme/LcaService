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
    public class FinalizeOrderWorkflowManager : BackgroundService
    {
        private readonly ILogger<FinalizeOrderWorkflowManager> _logger;
        private readonly IQueueProvider _inprogressQueueProvider;
        private readonly IAcmeProvider _acmeProvider;
        private readonly IOrderStorageService _orderStorageService;

        public FinalizeOrderWorkflowManager(ILogger<FinalizeOrderWorkflowManager> logger, IAcmeProvider acmeProvider, IOrderStorageService orderStorageService)
        {
            _logger = logger;
            _acmeProvider = acmeProvider;
            _orderStorageService = orderStorageService;
            _inprogressQueueProvider = new QueueProvider("inprogress");

        }

        protected  async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           await Dowork(stoppingToken);
            
        }

        private async Task Dowork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (await _inprogressQueueProvider.Count() != 0)
                    {
                        var queueMessage = await _inprogressQueueProvider.GetMessage();

                        if (queueMessage != null && queueMessage.MessageText != null)
                        {
                            var order = JsonSerializer.Deserialize<Order>(queueMessage.MessageText);
                            order = await _orderStorageService.RetrieveAsync(order.Id);

                            var certificate = await _acmeProvider.Download(order.link, new Certes.CsrInfo { CommonName = order.SAN });

                            order.Status = "Finished";
                            order.Certificate = certificate;

                            await _orderStorageService.UpdateEntity(order);
                            await _inprogressQueueProvider.Delete(queueMessage);
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
