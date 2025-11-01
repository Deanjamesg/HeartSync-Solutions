using HeartSyncSolutions.Models;
using HeartSyncSolutions.Services;
using HeartSyncSolutions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeartSyncSolutions.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> AccountDetails()
        {
            // Get the current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch user data from UserService
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found in database.");
                return RedirectToAction("Login", "Account");
            }

            // Get additional statistics
            var eventsAttended = user.UserEvents?.Count(ue => ue.AttendanceStatusID != null) ?? 0;
            var totalDonations = await _userService.GetUserTotalDonationAmountAsync(userId);

            // Pass data to the view
            ViewBag.EventsAttended = eventsAttended;
            ViewBag.TotalDonations = totalDonations;

            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_AccountDetails", user);
            }

            return View("_AccountDetails", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInfo(string firstName, string lastName, string email, string phone)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found" });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found in database" });
            }

            // Update user properties
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.ContactNumber = phone;

            var success = await _userService.UpdateUserAsync(user);

            if (success)
            {
                _logger.LogInformation($"User {userId} updated their personal information successfully.");
                
                if (Request.Headers["HX-Request"] == "true")
                {
                    return await AccountDetails(); // Return updated view
                }
                
                return Json(new { success = true, message = "Personal information updated successfully!" });
            }

            return Json(new { success = false, message = "Failed to update personal information" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([FromForm] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning($"Password update validation failed: {string.Join(", ", errors)}");
                
                return Json(new { success = false, message = string.Join(" ", errors) });
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found during password update.");
                return Json(new { success = false, message = "User not found" });
            }

            // Change the password using UserManager
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // Refresh the sign-in cookie to prevent logout after password change
                await _signInManager.RefreshSignInAsync(user);
                
                _logger.LogInformation($"User {user.Email} changed their password successfully.");
                
                return Json(new { success = true, message = "Password updated successfully! You can now use your new password to log in." });
            }

            // Log the errors
            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning($"Password update failed for user {user.Email}: {string.Join(", ", errorMessages)}");

            return Json(new { success = false, message = string.Join(" ", errorMessages) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacySettings(bool showDonations)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Add privacy settings logic here when you have a privacy settings field in your model
            _logger.LogInformation($"User {userId} updated privacy settings. Show donations: {showDonations}");

            return Json(new { success = true, message = "Privacy settings updated successfully!" });
        }
    }
}
