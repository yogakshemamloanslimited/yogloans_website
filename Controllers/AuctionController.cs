using Microsoft.AspNetCore.Mvc;

namespace yogloansdotnet.Controllers
{
    public class AuctionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            return View ("~/Views/Auction/login.cshtml");
        }
    }
}
