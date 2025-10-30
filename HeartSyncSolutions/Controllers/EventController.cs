using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    public class EventController : Controller
    {
        public IActionResult EventListPartial()
        {
            return PartialView("_EventList");
        }

        public IActionResult CreateEventPartial()
        {
            return PartialView("_CreateEvent");
        }

        public IActionResult EventDetailsPartial()
        {
            return PartialView("_EventDetails");
        }

        public IActionResult SignedUpEventsPartial()
        {
            return PartialView("_SignedUpEvents");
        }
    }
}
