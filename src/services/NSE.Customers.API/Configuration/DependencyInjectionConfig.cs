using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Mediator;
using NSE.Customers.API.Application.Commands;

namespace NSE.Customers.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<RegistarClienteCommand, ValidationResult>, ClienteCommandHandler>();
        }
    }
}