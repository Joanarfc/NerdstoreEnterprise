using FluentValidation;
using NSE.Core.Messages;
using System;

namespace NSE.Customers.API.Application.Commands
{
    public class RegistarClienteCommand : Command
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }
        public RegistarClienteCommand(Guid id, string nome, string email, string cpf)
        {
            AggregateId = id;
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
        }
        public override bool IsValido()
        {
            ValidationResult = new RegistarClienteValidation().Validate(this);
            return ValidationResult.IsValid;
        }
        public class RegistarClienteValidation : AbstractValidator<RegistarClienteCommand>
        {
            public RegistarClienteValidation()
            {
                RuleFor(c => c.Id)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Id do cliente inválido!");

                RuleFor(c => c.Cpf)
                    .Must(TerCpfValido)
                    .WithMessage("O CPF informado não é válido!");

                RuleFor(c => c.Email)
                    .Must(TerEmailValido)
                    .WithMessage("O Email informado não é válido!");
            }
            protected static bool TerCpfValido(string cpf)
            {
                return Core.DomainObjects.Cpf.Validar(cpf);
            }

            protected static bool TerEmailValido(string email)
            {
                return Core.DomainObjects.Email.Validar(email);

            }
        }
    }
}