using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    [AllowAnonymous]
    public class DonationController : Controller
    {
        private readonly ILogger<DonationController> _logger;

        public DonationController(ILogger<DonationController> logger)
        {
            _logger = logger;
        }

        public IActionResult Donate()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_Donate");
            }

            return View("_Donate");
        }

        public IActionResult SubmitDonation()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
            }

            return RedirectToAction("DonationHistory");
        }

        public IActionResult RequestInKindDonation()
        {
            return RedirectToAction("DonationHistory");
        }

        public IActionResult DonationHistory()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_DonationHistory");
            }

            return View("_DonationHistory");
        }

        public IActionResult DonationsList()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_DonationsList");
            }

            return View("_DonationsList");
        }

        public IActionResult DonationRequests()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_DonationRequests");
            }

            return View("_Donate");
        }

    }
}
