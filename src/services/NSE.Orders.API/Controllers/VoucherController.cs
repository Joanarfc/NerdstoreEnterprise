using Microsoft.AspNetCore.Authorization;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Orders.API.Controllers
{
    [Authorize]
    public class VoucherController : MainController
    {
    }
}