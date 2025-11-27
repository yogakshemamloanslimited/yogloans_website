using Microsoft.AspNetCore.Mvc;

namespace yogloansdotnet.Controllers
{
    public class _500Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
