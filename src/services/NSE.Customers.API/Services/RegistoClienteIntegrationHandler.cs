using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.Customers.API.Application.Commands;
using NSE.MessageBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Customers.API.Services
{
    public class RegistoClienteIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistoClienteIntegrationHandler(IServiceProvider serviceProvider, 
                                                IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        private void SetResponder()
        {
            _bus.RespondAsync<UtilizadorRegistadoIntegrationEvent, ResponseMessage>(
                async request => await RegistarCliente(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
         {
            SetResponder();

            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }
        private async Task<ResponseMessage> RegistarCliente(UtilizadorRegistadoIntegrationEvent message)
        {
            var clienteCommand = new RegistarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);

            ValidationResult sucesso;

            using(var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                sucesso = await mediator.EnviarComando(clienteCommand);
            }

            return new ResponseMessage(sucesso);
        }
    }
}