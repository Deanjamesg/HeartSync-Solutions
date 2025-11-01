using HeartSyncSolutions.Extensions;
using HeartSyncSolutions.Services;
using HeartSyncSolutions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeartSyncSolutions.Controllers
{
    [AllowAnonymous]
    public class DonationController : Controller
    {
        private readonly ILogger<DonationController> _logger;
        private readonly IDonationService _donationService;
        private readonly IUserService _userService;

        public DonationController(
            ILogger<DonationController> logger,
            IDonationService donationService,
            IUserService userService)
        {
            _logger = logger;
            _donationService = donationService;
            _userService = userService;
        }

        // GET: /Donation/Donate
        [HttpGet]
        public IActionResult Donate()
        {
            var viewModel = new MonetaryDonationViewModel();

            if (User.Identity.IsAuthenticated)
            {
                viewModel.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_Donate", viewModel);
            }

            return View(viewModel);
        }

        // POST: /Donation/SubmitMonetaryDonation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMonetaryDonation(MonetaryDonationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Donate", viewModel);
            }

            try
            {
                // Get user ID if authenticated
                if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(viewModel.ApplicationUserId))
                {
                    viewModel.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                var donation = viewModel.ToModel();
                var result = await _donationService.CreateMonetaryDonationAsync(donation);

                _logger.LogInformation($"Monetary donation created: {result.MonetaryDonationID} - Amount: {result.DonationAmount:C}");

                TempData["SuccessMessage"] = "Thank you for your generous donation!";
                return RedirectToAction("DonationConfirmation", new { id = result.MonetaryDonationID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating monetary donation");
                ModelState.AddModelError("", "An error occurred while processing your donation. Please try again.");
                return View("Donate", viewModel);
            }
        }

        // GET: /Donation/DonateInKind
        [HttpGet]
        public IActionResult DonateInKind()
        {
            var viewModel = new InKindDonationViewModel();

            if (User.Identity.IsAuthenticated)
            {
                viewModel.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_DonateInKind", viewModel);
            }

            return View(viewModel);
        }

        // POST: /Donation/SubmitInKindDonation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitInKindDonation(InKindDonationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("DonateInKind", viewModel);
            }

            try
            {
                // Get user ID if authenticated
                if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(viewModel.ApplicationUserId))
                {
                    viewModel.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                var offer = viewModel.ToModel();
                var result = await _donationService.CreateInKindOfferAsync(offer);

                _logger.LogInformation($"In-kind offer created: {result.InKindOfferID} - Item: {result.ItemDescription}");

                TempData["SuccessMessage"] = "Thank you for your in-kind donation offer! We'll contact you soon.";
                return RedirectToAction("DonationConfirmation", new { id = result.InKindOfferID, isInKind = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating in-kind donation");
                ModelState.AddModelError("", "An error occurred while processing your donation. Please try again.");
                return View("DonateInKind", viewModel);
            }
        }

        // GET: /Donation/DonationConfirmation
        [HttpGet]
        public IActionResult DonationConfirmation(string id, bool isInKind = false)
        {
            ViewBag.DonationId = id;
            ViewBag.IsInKind = isInKind;
            return View();
        }

        // GET: /Donation/DonationHistory
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DonationHistory()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userService.GetUserByIdAsync(userId);

                var monetaryDonations = await _donationService.GetMonetaryDonationsByUserAsync(userId);
                var inKindOffers = await _donationService.GetInKindOffersByUserAsync(userId);

                var viewModel = new DonationHistoryViewModel
                {
                    UserId = userId,
                    UserName = $"{user?.FirstName} {user?.LastName}",
                    MonetaryDonations = monetaryDonations.ToViewModelList().ToList(),
                    InKindDonations = inKindOffers.ToViewModelList().ToList(),
                    TotalMonetaryDonations = await _donationService.GetTotalMonetaryDonationsByUserAsync(userId),
                    TotalInKindDonations = inKindOffers.Count(),
                    TotalDonationCount = monetaryDonations.Count() + inKindOffers.Count(),
                    FirstDonationDate = monetaryDonations.Any() || inKindOffers.Any()
                        ? new[] { monetaryDonations.Select(d => d.Date).DefaultIfEmpty().Min(), inKindOffers.Select(o => o.OfferedDate).DefaultIfEmpty().Min() }.Min()
                        : (DateTime?)null,
                    LastDonationDate = monetaryDonations.Any() || inKindOffers.Any()
                        ? new[] { monetaryDonations.Select(d => d.Date).DefaultIfEmpty().Max(), inKindOffers.Select(o => o.OfferedDate).DefaultIfEmpty().Max() }.Max()
                        : (DateTime?)null
                };

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_DonationHistory", viewModel);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donation history");
                return View("Error");
            }
        }

        // GET: /Donation/DonationsList (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DonationsList(string filterStatus = null, string filterType = null, string sortBy = "date")
        {
            try
            {
                var monetaryDonations = await _donationService.GetAllMonetaryDonationsAsync();
                var inKindOffers = await _donationService.GetAllInKindOffersAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(filterStatus))
                {
                    inKindOffers = inKindOffers.Where(o => o.Status == filterStatus);
                }

                if (!string.IsNullOrEmpty(filterType))
                {
                    if (filterType == "monetary")
                    {
                        inKindOffers = Enumerable.Empty<Models.InKindOffer>();
                    }
                    else if (filterType == "inkind")
                    {
                        monetaryDonations = Enumerable.Empty<Models.MonetaryDonations>();
                    }
                }

                var viewModel = new DonationListViewModel
                {
                    MonetaryDonations = monetaryDonations.ToViewModelList().ToList(),
                    InKindDonations = inKindOffers.ToViewModelList().ToList(),
                    TotalMonetaryAmount = await _donationService.GetTotalMonetaryDonationsAsync(),
                    TotalMonetaryCount = monetaryDonations.Count(),
                    TotalInKindCount = inKindOffers.Count(),
                    TotalDonationCount = monetaryDonations.Count() + inKindOffers.Count(),
                    FilterStatus = filterStatus,
                    FilterType = filterType,
                    SortBy = sortBy
                };

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_DonationsList", viewModel);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donations list");
                return View("Error");
            }
        }

        // GET: /Donation/Statistics (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var monetaryDonations = await _donationService.GetAllMonetaryDonationsAsync();
                var inKindOffers = await _donationService.GetAllInKindOffersAsync();
                var recentMonetary = await _donationService.GetRecentMonetaryDonationsAsync(5);
                var recentInKind = await _donationService.GetRecentInKindOffersAsync(5);

                var viewModel = new DonationStatisticsViewModel
                {
                    TotalMonetaryDonations = await _donationService.GetTotalMonetaryDonationsAsync(),
                    TotalMonetaryDonationCount = monetaryDonations.Count(),
                    TotalInKindDonationCount = inKindOffers.Count(),
                    TotalDonationCount = await _donationService.GetTotalDonationCountAsync(),
                    AverageDonationAmount = monetaryDonations.Any() ? monetaryDonations.Average(d => d.DonationAmount) : 0,
                    LargestDonation = monetaryDonations.Any() ? monetaryDonations.Max(d => d.DonationAmount) : 0,
                    TotalDonors = monetaryDonations.Select(d => d.ApplicationUserID).Distinct().Count() + 
                                  inKindOffers.Select(o => o.ApplicationUserID).Distinct().Count(),
                    AnonymousDonationCount = monetaryDonations.Count(d => d.IsAnonymous),
                    RecentMonetaryDonations = recentMonetary.ToViewModelList().ToList(),
                    RecentInKindDonations = recentInKind.ToViewModelList().ToList(),
                    // Group by month
                    DonationsByMonth = monetaryDonations
                        .GroupBy(d => d.Date.ToString("yyyy-MM"))
                        .ToDictionary(g => g.Key, g => g.Count()),
                    MonetaryDonationsByMonth = monetaryDonations
                        .GroupBy(d => d.Date.ToString("yyyy-MM"))
                        .ToDictionary(g => g.Key, g => g.Sum(d => d.DonationAmount)),
                    InKindDonationsByStatus = inKindOffers
                        .GroupBy(o => o.Status)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donation statistics");
                return View("Error");
            }
        }

        // POST: /Donation/UpdateInKindStatus (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInKindStatus(string offerId, string newStatus)
        {
            try
            {
                var result = await _donationService.UpdateInKindOfferStatusAsync(offerId, newStatus);

                if (result)
                {
                    TempData["SuccessMessage"] = "Status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update status.";
                }

                return RedirectToAction("DonationsList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating in-kind offer status");
                TempData["ErrorMessage"] = "An error occurred while updating the status.";
                return RedirectToAction("DonationsList");
            }
        }

        // GET: /Donation/Details/{id}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(string id, bool isInKind = false)
        {
            try
            {
                if (isInKind)
                {
                    var offer = await _donationService.GetInKindOfferByIdAsync(id);
                    if (offer == null)
                    {
                        return NotFound();
                    }
                    return View("InKindDetails", offer.ToViewModel());
                }
                else
                {
                    var donation = await _donationService.GetMonetaryDonationByIdAsync(id);
                    if (donation == null)
                    {
                        return NotFound();
                    }
                    return View("MonetaryDetails", donation.ToViewModel());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donation details");
                return View("Error");
            }
        }
    }
}
