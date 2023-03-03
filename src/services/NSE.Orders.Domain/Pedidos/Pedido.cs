using NSE.Core.DomainObjects;
using NSE.Orders.Domain.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSE.Orders.Domain.Pedidos
{
    public class Pedido : Entity, IAggregateRoot
    {
        public Pedido(Guid clienteId, decimal valorTotal, List<PedidoItem> pedidoItens,
            bool voucherUtilizado = false, decimal desconto = 0, Guid? voucherId = null)
        {
            ClienteId = clienteId;
            ValorTotal = valorTotal;
            _pedidoItens = pedidoItens;

            Desconto = desconto;
            VoucherUtilizado = voucherUtilizado;
            VoucherId = voucherId;
        }

        // EF ctor
        protected Pedido() { }

        public int Codigo { get; private set; }
        public Guid ClienteId { get; private set; }
        public Guid? VoucherId { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public decimal Desconto { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }

        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;
        public Endereco Endereco { get; private set; }

        // Rel EF:
        public Voucher Voucher { get; private set; }
        public void AutorizarPedido()
        {
            PedidoStatus = PedidoStatus.Autorizado;
        }
        public void CancelarPedido()
        {
            PedidoStatus = PedidoStatus.Cancelado;
        }

        public void FinalizarPedido()
        {
            PedidoStatus = PedidoStatus.Pago;
        }

        public void AtribuirVoucher(Voucher voucher)
        {
            VoucherUtilizado = true;
            VoucherId = voucher.Id;
            Voucher = voucher;
        }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void CalcularValorPedido()
        {
            ValorTotal = PedidoItens.Sum(p => p.CalcularValor());
            CalcularValorTotalDesconto();
        }

        public void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher.TipoDesconto == TipoDescontoVoucher.Percentagem)
            {
                if (Voucher.Percentual.HasValue)
                {
                    desconto = (valor * Voucher.Percentual.Value) / 100;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
        }
    }
}