using NSE.Payments.API.Models;
using System.Threading.Tasks;
using NSE.Core.Messages.Integration;
using System;

namespace NSE.Payments.API.Services
{
    public interface IPagamentoService
    {
        Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
        Task<ResponseMessage> CapturarPagamento(Guid pedidoId);
        Task<ResponseMessage> CancelarPagamento(Guid pedidoId);
    }
}