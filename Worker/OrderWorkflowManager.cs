using LcaService.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LcaService.Worker
{
    public class IncomingOrderWorkflowManager : BackgroundService
    {
        private readonly ILogger<IncomingOrderWorkflowManager> _logger;
        private readonly IQueueProvider _queueProvider;
        private readonly IQueueProvider _inprogressQueueProvider;
        private readonly IQueueProvider _tobeStartedQueueProvider;

        public IncomingOrderWorkflowManager(ILogger<IncomingOrderWorkflowManager> logger)
        {
            _logger = logger;
            _queueProvider = new QueueProvider("incoming");
            _inprogressQueueProvider = new QueueProvider("inprogress");
            _tobeStartedQueueProvider = new QueueProvider("tobestarted");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await  Dowork(stoppingToken);
        }

        private async Task Dowork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    if (await _queueProvider.Count() != 0 && await _inprogressQueueProvider.Count() < 5)
                    {
                        var queueMessage = await _queueProvider.GetMessage();
                        if (queueMessage != null && queueMessage.MessageText != null)
                        {
                            await _tobeStartedQueueProvider.SendMessage(queueMessage.MessageText);
                            await _queueProvider.Delete(queueMessage);
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
