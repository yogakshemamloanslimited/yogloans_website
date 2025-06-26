using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;

namespace yogloansdotnet.Controllers
{
    public class InvestorController : Controller
    {
        private readonly ILogger<InvestorController> _logger;

        public InvestorController(ILogger<InvestorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("investor")]
        [Route("investor/index")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
