using HeartSyncSolutions.Models;
using HeartSyncSolutions.Services;
using HeartSyncSolutions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly ILogger<DonationController> _logger;
        private readonly DonationService _donationService;
        private readonly UserManager<User> _userManager;

        public DonationController(
            ILogger<DonationController> logger,
            DonationService donationService,
            UserManager<User> userManager)
        {
            _logger = logger;
            _donationService = donationService;
            _userManager = userManager;
        }

        // GET: Donate Page
        [Authorize]
        public IActionResult Donate()
        {
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_Donate", new MonetaryDonationViewModel());
            }

            return View("_Donate", new MonetaryDonationViewModel());
        }

        // POST: Submit Monetary Donation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDonation(MonetaryDonationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to submit donation");
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for monetary donation");

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_Donate", model);
                }

                return View("_Donate", model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("Could not find user for donation submission");
                    TempData["Error"] = "Unable to process donation. Please try logging in again.";
                    return RedirectToAction("Login", "Account");
                }

                var success = await _donationService.CreateMonetaryDonationAsync(model, user.Id);

                if (success)
                {
                    _logger.LogInformation($"Monetary donation of R{model.Amount} created successfully for user {user.Email}");
                    TempData["Success"] = $"Thank you for your donation of R{model.Amount:N2}! Your contribution makes a difference.";

                    if (Request.Headers["HX-Request"] == "true")
                    {
                        return await DonationHistory();
                    }

                    return RedirectToAction("DonationHistory");
                }
                else
                {
                    _logger.LogError("Failed to create monetary donation");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your donation. Please try again.");

                    if (Request.Headers["HX-Request"] == "true")
                    {
                        return PartialView("_Donate", model);
                    }

                    return View("_Donate", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting monetary donation");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_Donate", model);
                }

                return View("_Donate", model);
            }
        }

        // POST: Submit In-Kind Donation Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestInKindDonation(InKindDonationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to submit in-kind donation");
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for in-kind donation");
                TempData["Error"] = "Please fill in all required fields.";

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_Donate", new MonetaryDonationViewModel());
                }

                return RedirectToAction("Donate");
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("Could not find user for in-kind donation submission");
                    TempData["Error"] = "Unable to process donation request. Please try logging in again.";
                    return RedirectToAction("Login", "Account");
                }

                var success = await _donationService.CreateInKindDonationAsync(model, user.Id);

                if (success)
                {
                    _logger.LogInformation($"In-kind donation request created successfully for user {user.Email}");
                    TempData["Success"] = "Thank you! Your in-kind donation request has been submitted. We'll contact you soon to arrange collection.";

                    if (Request.Headers["HX-Request"] == "true")
                    {
                        return await DonationHistory();
                    }

                    return RedirectToAction("DonationHistory");
                }
                else
                {
                    _logger.LogError("Failed to create in-kind donation");
                    TempData["Error"] = "An error occurred while processing your donation request. Please try again.";
                    return RedirectToAction("Donate");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting in-kind donation");
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction("Donate");
            }
        }

        // GET: Donation History
        public async Task<IActionResult> DonationHistory()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var statistics = await _donationService.GetUserDonationStatisticsAsync(user.Id);
                var monetaryDonations = await _donationService.GetUserMonetaryDonationsAsync(user.Id);
                var inKindDonations = await _donationService.GetUserInKindDonationsAsync(user.Id);

                var viewModel = new DonationHistoryViewModel
                {
                    TotalDonated = statistics.TotalMonetaryDonated,
                    TotalDonations = statistics.TotalMonetaryDonations + statistics.TotalInKindDonations,
                    MonthlyRecurring = statistics.MonthlyRecurringAmount,
                    InKindDonationsCount = statistics.TotalInKindDonations,
                    MonetaryDonations = monetaryDonations,
                    InKindDonations = inKindDonations
                };

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_DonationHistory", viewModel);
                }

                return View("_DonationHistory", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading donation history");
                TempData["Error"] = "An error occurred while loading your donation history.";
                return RedirectToAction("Index", "Home");
            }
        }

        // ADMIN: View All Donations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DonationsList()
        {
            try
            {
                var allMonetaryDonations = await _donationService.GetAllMonetaryDonationsAsync();
                var allInKindDonations = await _donationService.GetAllInKindDonationsAsync();

                var viewModel = new AdminDonationsViewModel
                {
                    MonetaryDonations = allMonetaryDonations,
                    InKindDonations = allInKindDonations
                };

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_DonationsList", viewModel);
                }

                return View("_DonationsList", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading donations list");
                TempData["Error"] = "An error occurred while loading the donations list.";
                return RedirectToAction("Index", "Home");
            }
        }

        // ADMIN: View Pending Donation Requests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DonationRequests()
        {
            try
            {
                var allInKindDonations = await _donationService.GetAllInKindDonationsAsync();

                var viewModel = new AdminInKindDonationsViewModel
                {
                    InKindDonations = allInKindDonations
                        .Select(d => InKindDonationItem.FromInKindDonation(d))
                        .ToList()
                };

                if (Request.Headers["HX-Request"] == "true")
                {
                    return PartialView("_DonationRequests", viewModel);
                }

                return View("_DonationRequests", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading donation requests");
                TempData["Error"] = "An error occurred while loading donation requests.";
                return RedirectToAction("Index", "Home");
            }
        }

        // ADMIN: Approve In-Kind Donation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveInKindDonation(string donationId)
        {
            try
            {
                var approvedStatus = await _donationService.GetInKindStatusByNameAsync("Approved");

                if (approvedStatus == null)
                {
                    TempData["Error"] = "Approved status not found in system.";
                    return RedirectToAction("DonationRequests");
                }

                var success = await _donationService.UpdateInKindDonationStatusAsync(donationId, approvedStatus.InKindStatusID);

                if (success)
                {
                    TempData["Success"] = "In-kind donation request has been approved successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to approve donation request.";
                }

                return RedirectToAction("DonationRequests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving in-kind donation {donationId}");
                TempData["Error"] = "An error occurred while approving the donation.";
                return RedirectToAction("DonationRequests");
            }
        }

        // ADMIN: Decline In-Kind Donation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineInKindDonation(string donationId)
        {
            try
            {
                var cancelledStatus = await _donationService.GetInKindStatusByNameAsync("Cancelled");

                if (cancelledStatus == null)
                {
                    TempData["Error"] = "Cancelled status not found in system.";
                    return RedirectToAction("DonationRequests");
                }

                var success = await _donationService.UpdateInKindDonationStatusAsync(donationId, cancelledStatus.InKindStatusID);

                if (success)
                {
                    TempData["Success"] = "In-kind donation request has been declined.";
                }
                else
                {
                    TempData["Error"] = "Failed to decline donation request.";
                }

                return RedirectToAction("DonationRequests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error declining in-kind donation {donationId}");
                TempData["Error"] = "An error occurred while declining the donation.";
                return RedirectToAction("DonationRequests");
            }
        }

        // ADMIN: Complete In-Kind Donation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteInKindDonation(string donationId)
        {
            try
            {
                var completedStatus = await _donationService.GetInKindStatusByNameAsync("Completed");

                if (completedStatus == null)
                {
                    TempData["Error"] = "Completed status not found in system.";
                    return RedirectToAction("DonationRequests");
                }

                var success = await _donationService.UpdateInKindDonationStatusAsync(donationId, completedStatus.InKindStatusID);

                if (success)
                {
                    TempData["Success"] = "In-kind donation marked as completed.";
                }
                else
                {
                    TempData["Error"] = "Failed to mark donation as completed.";
                }

                return RedirectToAction("DonationRequests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing in-kind donation {donationId}");
                TempData["Error"] = "An error occurred while completing the donation.";
                return RedirectToAction("DonationRequests");
            }
        }

        // ADMIN: Complete Monetary Donation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteMonetaryDonation(string donationId)
        {
            try
            {
                var completedStatus = await _donationService.GetMonetaryDonationStatusByNameAsync("Completed");

                if (completedStatus == null)
                {
                    TempData["Error"] = "Completed status not found in system.";
                    return RedirectToAction("DonationsList");
                }

                var success = await _donationService.UpdateMonetaryDonationStatusAsync(donationId, completedStatus.MonetaryDonationStatusID);

                if (success)
                {
                    TempData["Success"] = "Monetary donation marked as completed.";
                }
                else
                {
                    TempData["Error"] = "Failed to mark donation as completed.";
                }

                return RedirectToAction("DonationsList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing monetary donation {donationId}");
                TempData["Error"] = "An error occurred while completing the donation.";
                return RedirectToAction("DonationsList");
            }
        }
    }
}