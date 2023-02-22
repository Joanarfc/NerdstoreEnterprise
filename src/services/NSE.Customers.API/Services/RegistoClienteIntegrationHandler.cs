using EasyNetQ;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.Customers.API.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Customers.API.Services
{
    public class RegistoClienteIntegrationHandler : BackgroundService
    {
        private IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistoClienteIntegrationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus = RabbitHutch.CreateBus("host=localhost:5672");

            _bus.Rpc.RespondAsync<UtilizadorRegistadoIntegrationEvent, ResponseMessage>(
                async request => new ResponseMessage(await RegistarCliente(request)));

            return Task.CompletedTask;
        }
        private async Task<ValidationResult> RegistarCliente(UtilizadorRegistadoIntegrationEvent message)
        {
            var clienteCommand = new RegistarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);

            ValidationResult sucesso;

            using(var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();

                sucesso = await mediator.EnviarComando(clienteCommand);
            }

            return sucesso;
        }
    }
}