using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using NSE.Customers.API.Application.Events;
using NSE.Customers.API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Customers.API.Application.Commands
{
    public class ClienteCommandHandler : CommandHandler,
                                         IRequestHandler<RegistarClienteCommand, ValidationResult>
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ValidationResult> Handle(RegistarClienteCommand message, CancellationToken cancellationToken)
        {
            // Validate command
            if (!message.IsValido()) return message.ValidationResult;

            // Create the entity instance
            var cliente = new Cliente(message.Id, message.Nome, message.Email, message.Cpf);

            // Business validations
            var clienteExistente = await _clienteRepository.ObterPorCpf(cliente.Cpf.Numero);

            if (clienteExistente != null)
            {
                AdicionarErro("Este CPF já está em uso");
                return ValidationResult;
            }

            // Persist in the database
            _clienteRepository.Adicionar(cliente);

            cliente.AdicionarEvento(new ClienteRegistadoEvent(message.Id, message.Nome, message.Email, message.Cpf));

            return await PersistirDados(_clienteRepository.UnitOfWork);
        }
    }
}