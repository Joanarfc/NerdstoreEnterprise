using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NSE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        public static string HashEmailForGravatar(this RazorPage page, string email)
        {
            /* Create an instance of the MD5 hash algorithm  */
            var md5Hasher = MD5.Create();

            /* Compute the hash value of the email by converting the email string to a byte array */
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

            /* Create an empty StringBuilder object to store the hexadecimal representation of the hash value */
            var sBuilder = new StringBuilder();

            /* Iterate through the byte array "data" 
             * and append each byte in hexadecimal format to the "sBuilder" object */
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            /* Return the string representation of the hash value */
            return sBuilder.ToString();
        }
        public static string FormatoMoeda(this RazorPage page, decimal valor)
        {
            return FormatoMoeda(valor);
        }
        private static string FormatoMoeda(decimal valor)
        {
            return string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", valor);
        }
        public static string MensagemStock(this RazorPage page, int quantidade)
        {
            return quantidade > 0 ? $"Apenas {quantidade} em stock!" : "Produto esgotado";
        }
        public static string UnidadesPorProduto(this RazorPage page, int unidades)
        {
            return unidades > 1 ? $"{unidades} unidades" : $"{unidades} unidade";
        }
        public static string UnidadesPorProdutoValorTotal(this RazorPage page, int unidades, decimal valor)
        {
            return $"{unidades}x {FormatoMoeda(valor)} = Total: {FormatoMoeda(valor * unidades)}";
        }
        public static string SelectOptionsPorQuantidade(this RazorPage page, int quantidade, int valorSelecionado = 0)
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= quantidade; i++)
            {
                var selected = "";
                if (i == valorSelecionado) selected = "selected";
                sb.Append($"<option {selected} value='{i}'>{i}</option>");
            }

            return sb.ToString();
        }
        public static string ExibeStatus(this RazorPage page, int status)
        {
            var statusMensagem = "";
            var statusClasse = "";

            switch (status)
            {
                case 1:
                    statusClasse = "info";
                    statusMensagem = "Em aprovação";
                    break;
                case 2:
                    statusClasse = "primary";
                    statusMensagem = "Aprovado";
                    break;
                case 3:
                    statusClasse = "danger";
                    statusMensagem = "Recusado";
                    break;
                case 4:
                    statusClasse = "success";
                    statusMensagem = "Entregue";
                    break;
                case 5:
                    statusClasse = "warning";
                    statusMensagem = "Cancelado";
                    break;

            }

            return $"<span class='badge badge-{statusClasse}'>{statusMensagem}</span>";
        }
    }
}