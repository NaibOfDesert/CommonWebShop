using CommonWebShopDependencyInjection.Models;
using CommonWebShopDependencyInjection.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace CommonWebShopDependencyInjection.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;
        private readonly ITransientGuidService _transient1;
        private readonly ITransientGuidService _transient2;

        private readonly IScopedGuidService _scoped1;
        private readonly IScopedGuidService _scoped2;

        private readonly ISingeltonGuidService _singelton1;
        private readonly ISingeltonGuidService _singelton2;



        public HomeController(ITransientGuidService transient1,
            ITransientGuidService transient2,
            IScopedGuidService scopedGuid1, 
            IScopedGuidService scopedGuid2,
            ISingeltonGuidService singelton1,
            ISingeltonGuidService singelton2)
        {
            _transient1 = transient1;
            _transient2 = transient2;
            _scoped1 = scopedGuid1;
            _scoped2 = scopedGuid2;
            _singelton1 = singelton1;
            _singelton2 = singelton2;

        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public IActionResult Index()
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Transient 1: {_transient1.GetGuid()}\n");
            message.Append($"Transient 2: {_transient2.GetGuid()}\n\n\n");
            message.Append($"Scoped 1: {_scoped1.GetGuid()}\n");
            message.Append($"Scoped 2: {_scoped2.GetGuid()}\n\n\n");
            message.Append($"Singelton 1: {_singelton1.GetGuid()}\n");
            message.Append($"Singelton 2: {_singelton2.GetGuid()}\n\n\n");


            return Ok(message.ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}