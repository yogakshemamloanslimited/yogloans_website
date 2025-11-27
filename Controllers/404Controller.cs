using Microsoft.AspNetCore.Mvc;

namespace yogloansdotnet.Controllers
{
    public class _404Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
