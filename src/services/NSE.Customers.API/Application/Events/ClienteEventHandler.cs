using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Customers.API.Application.Events
{
    public class ClienteEventHandler : INotificationHandler<ClienteRegistadoEvent>
    {
        public Task Handle(ClienteRegistadoEvent notification, CancellationToken cancellationToken)
        {
            // Send confirmation event

            return Task.CompletedTask;
        }
    }
}