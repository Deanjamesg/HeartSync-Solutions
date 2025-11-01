using System.Diagnostics;
using HeartSyncSolutions.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Check if this is an HTMX request
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("Index");
            }
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
