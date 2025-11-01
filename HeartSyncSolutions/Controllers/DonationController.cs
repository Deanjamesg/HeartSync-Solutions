using HeartSyncSolutions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly ILogger<DonationController> _logger;

        public DonationController(ILogger<DonationController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Donate()
        {
            return PartialView("_Donate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> SubmitDonation(MonetaryDonationViewModel model)
        public IActionResult SubmitDonation(MonetaryDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("------ TEST ------");
                return View("_Donate", model);
            }

            return RedirectToAction("DonationHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> RequestInKindDonation(InKindDonationViewModel model)
        public IActionResult RequestInKindDonation(InKindDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("_Donate", model);
            }

            return RedirectToAction("DonationHistory");
        }

        //public IActionResult Donate()
        //{
        //    if (Request.Headers["HX-Request"] == "true")
        //    {
        //        return PartialView("_Donate");
        //    }

        //    return View("_Donate");
        //}

        public IActionResult SubmitDonation()
        {
            Console.WriteLine("------ TEST ------");
            if (Request.Headers["HX-Request"] == "true")
            {
            }

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

        [Authorize(Roles = "Admin")]
        public IActionResult DonationsList()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_DonationsList");
            }

            return View("_DonationsList");
        }

        [Authorize(Roles = "Admin")]
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
