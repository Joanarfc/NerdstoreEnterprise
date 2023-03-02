using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.Payments.API.Data;
using NSE.Payments.API.Data.Repository;
using NSE.Payments.API.Facade;
using NSE.Payments.API.Models;
using NSE.Payments.API.Services;
using NSE.WebAPI.Core.Utilizador;

namespace NSE.Payments.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            //API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            //Services
            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();

            //Data
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<PagamentosContext>();
        }
    }
}