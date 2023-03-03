using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Catalog.API.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Catalog.API.Services
{
    public class CatalogoIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public CatalogoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<PedidoAutorizadoIntegrationEvent>("PedidoAutorizado", async request =>
                await BaixarStock(request));
        }

        private async Task BaixarStock(PedidoAutorizadoIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var produtosComStock = new List<Produto>();
                var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

                var idsProdutos = string.Join(",", message.Itens.Select(c => c.Key));
                var produtos = await produtoRepository.ObterProdutosPorId(idsProdutos);

                if (produtos.Count != message.Itens.Count)
                {
                    CancelarPedidoSemStock(message);
                    return;
                }

                foreach (var produto in produtos)
                {
                    var quantidadeProduto = message.Itens.FirstOrDefault(p => p.Key == produto.Id).Value;

                    if (produto.EstaDisponivel(quantidadeProduto))
                    {
                        produto.RetirarStock(quantidadeProduto);
                        produtosComStock.Add(produto);
                    }
                }

                if (produtosComStock.Count != message.Itens.Count)
                {
                    CancelarPedidoSemStock(message);
                    return;
                }

                foreach (var produto in produtosComStock)
                {
                    produtoRepository.Atualizar(produto);
                }

                if (!await produtoRepository.UnitOfWork.Commit())
                {
                    throw new DomainException($"Problemas ao atualizar Stock do pedido {message.PedidoId}");
                }

                var pedidoBaixado = new PedidoBaixadoStockIntegrationEvent(message.ClienteId, message.PedidoId);
                await _bus.PublishAsync(pedidoBaixado);
            }
        }

        public async void CancelarPedidoSemStock(PedidoAutorizadoIntegrationEvent message)
        {
            var pedidoCancelado = new PedidoCanceladoIntegrationEvent(message.ClienteId, message.PedidoId);
            await _bus.PublishAsync(pedidoCancelado);
        }
    }
}