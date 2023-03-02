using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSE.MessageBus;
using NSE.Orders.API.Application.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Orders.API.Services
{
    public class PedidoOrquestradorIntegrationHandler : IHostedService, IDisposable
    {
        private readonly ILogger<PedidoOrquestradorIntegrationHandler> _logger;
        private Timer _timer;

        public PedidoOrquestradorIntegrationHandler(ILogger<PedidoOrquestradorIntegrationHandler> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de pedidos iniciado.");

            _timer = new Timer(ProcessarPedidos, null, TimeSpan.Zero,
                               TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de pedidos finalizado.");

            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
        private async void ProcessarPedidos(object state)
        {
            _logger.LogInformation("Processando pedidos...");
        }
    }
}