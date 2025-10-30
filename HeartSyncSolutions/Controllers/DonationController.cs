using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    public class DonationController : Controller
    {
        [HttpGet]
        public IActionResult DonationPartial()
        {
            Console.WriteLine("DONATION PARTIAL");
            return PartialView("_Donation");
        }
    }
}
