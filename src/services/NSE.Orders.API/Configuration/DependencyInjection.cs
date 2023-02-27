using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Mediator;
using NSE.Orders.API.Application.Queries;
using NSE.Orders.Domain.Vouchers;
using NSE.Orders.Infra.Data;
using NSE.Orders.Infra.Data.Repository;
using NSE.WebAPI.Core.Utilizador;

namespace NSE.Orders.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            //API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            //Application
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IVoucherQueries, VoucherQueries>();

            //Data
            services.AddScoped<PedidosContext>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
        }
    }
}