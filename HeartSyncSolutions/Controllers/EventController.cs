using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        public IActionResult Events()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_Events");
            }

            return View("_Events");
        }

        public IActionResult EventDetails(string eventId)
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_EventDetails");
            }

            return View("_EventDetails");
        }

        public IActionResult SignedUpEvents()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_SignedUpEvents");
            }

            return View("_SignedUpEvents");
        }

        public IActionResult CreateEvent()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_CreateEvent");
            }

            return View("_CreateEvent");
        }

        public IActionResult EventManager()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_EventManager");
            }

            return View("_EventManager");
        }

        public IActionResult SignUp(string eventId)
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return Ok(new { success = true, message = "Successfully signed up for event!" });
            }

            return RedirectToAction("SignedUpEvents");
        }

        public IActionResult CancelRegistration(string eventId)
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_SignedUpEvents");
            }

            return RedirectToAction("SignedUpEvents");
        }

        public IActionResult Delete(string eventId)
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_EventManager");
            }

            return RedirectToAction("EventManager");
        }

        public IActionResult Edit(string eventId)
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_CreateEvent");
            }

            return View("_CreateEvent");
        }

    }
}
