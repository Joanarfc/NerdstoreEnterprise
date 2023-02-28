using MediatR;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using System.Threading.Tasks;
using System.Threading;

namespace NSE.Orders.API.Application.Events
{
    public class PedidoEventHandler : INotificationHandler<PedidoRealizadoEvent>
    {
        private readonly IMessageBus _bus;
        public PedidoEventHandler(IMessageBus bus)
        {
            _bus = bus;
        }
        public async Task Handle(PedidoRealizadoEvent message, CancellationToken cancellationToken)
        {
            await _bus.PublishAsync(new PedidoRealizadoIntegrationEvent(message.ClienteId));
        }
    }
}