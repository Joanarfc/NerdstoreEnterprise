using NSE.Payments.API.Models;
using System.Threading.Tasks;
using NSE.Core.Messages.Integration;

namespace NSE.Payments.API.Services
{
    public interface IPagamentoService
    {
        Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
    }
}