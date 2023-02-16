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
            return valor > 0? string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", valor) : "Gratuito";
        }
        public static string MensagemStock(this RazorPage page, int quantidade)
        {
            return quantidade > 0 ? $"Apenas {quantidade} em stock!" : "Produto esgotado";
        }
    }
}