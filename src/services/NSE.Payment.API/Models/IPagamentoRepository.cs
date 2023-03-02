using NSE.Core.Data;

namespace NSE.Payments.API.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        void AdicionarPagamento(Pagamento pagamento);
    }
}