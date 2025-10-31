using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        public IActionResult AccountDetails()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_AccountDetails");
            }

            return View("_AccountDetails");
        }

        public IActionResult UpdatePersonalInfo()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_AccountDetails");
            }

            return RedirectToAction("AccountDetails");
        }

        public IActionResult UpdatePassword()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return Ok(new { success = true, message = "Password updated successfully!" });
            }

            return RedirectToAction("AccountDetails");
        }

        public IActionResult UpdatePrivacySettings()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return Ok(new { success = true, message = "Privacy settings updated!" });
            }

            return RedirectToAction("AccountDetails");
        }

        public IActionResult ActivitySummary()
        {
            return PartialView("_AccountDetails");
        }
    }
}
