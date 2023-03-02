using NSE.Payments.API.Models;
using System.Threading.Tasks;

namespace NSE.Payments.API.Facade
{
    public interface IPagamentoFacade
    {
        Task<Transacao> AutorizarPagamento(Pagamento pagamento);
    }
}