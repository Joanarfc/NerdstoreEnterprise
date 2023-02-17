using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using NSE.Customers.API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Customers.API.Application.Commands
{
    public class ClienteCommandHandler : CommandHandler,
                                         IRequestHandler<RegistarClienteCommand, ValidationResult>
    {
        public async Task<ValidationResult> Handle(RegistarClienteCommand message, CancellationToken cancellationToken)
        {
            // Validate command
            if (!message.IsValido()) return message.ValidationResult;

            // Create the entity instance
            var cliente = new Cliente(message.Id, message.Nome, message.Email, message.Cpf);

            // Business validations
            if (true) // There's already a client with this CPF
            {
                AdicionarErro("Este CPF já está em uso");
                return ValidationResult;
            }

            // Persist in the database
        }
    }
}