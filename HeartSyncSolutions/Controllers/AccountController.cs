using HeartSyncSolutions.Models;
using HeartSyncSolutions.Services;
using HeartSyncSolutions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeartSyncSolutions.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Use UserService to check if user exists first
            var user = await _userService.GetUserByEmailAsync(model.Email);
            
            if (user == null)
            {
                // User doesn't exist - they need to register
                _logger.LogWarning($"Login attempt for non-existent email: {model.Email}");
                ModelState.AddModelError(string.Empty, "No account found with this email address. Please register first.");
                return View(model);
            }

            // User exists, now try to sign in using SignInManager
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true); // Enable lockout after failed attempts

            if (result.Succeeded)
            {
                // Add custom claims for the user
                await AddUserClaimsAsync(user);
                
                _logger.LogInformation($"User {model.Email} logged in successfully.");
                _logger.LogInformation($"   - User ID: {user.Id}");
                _logger.LogInformation($"   - Name: {user.FirstName} {user.LastName}");
                
                return RedirectToAction("Index", "Home");
            }


            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {model.Email} account locked out due to multiple failed login attempts.");
                ModelState.AddModelError(string.Empty, "Your account has been locked out due to multiple failed login attempts. Please try again in 5 minutes or reset your password.");
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning($"User {model.Email} login not allowed - email may not be confirmed.");
                ModelState.AddModelError(string.Empty, "Your email address has not been confirmed. Please check your email for a confirmation link.");
                return View(model);
            }

            // If we get here, the password was incorrect
            _logger.LogWarning($"Failed login attempt for {model.Email} - incorrect password.");
            ModelState.AddModelError(string.Empty, "Incorrect password. Please try again.");
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration model validation failed.");
                return View(model);
            }

            // Use UserService to check if user already exists
            var existingUser = await _userService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration attempt for existing email: {model.Email}");
                ModelState.AddModelError(string.Empty, "A user with this email already exists. Please login or use a different email.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ContactNumber = model.PhoneNumber ?? string.Empty,
                PhoneNumber = model.PhoneNumber,
                IsVolunteer = model.IsVolunteer,
                IsDonor = model.IsDonor,
                EmailConfirmed = true
            };

            _logger.LogInformation($"Attempting to create user: {model.Email}");

            // Use UserManager to create the user (handles password hashing and Identity setup)
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation($"✅ User {model.Email} created successfully with ID: {user.Id}");
                _logger.LogInformation($"   - Name: {user.FirstName} {user.LastName}");
                _logger.LogInformation($"   - IsVolunteer: {user.IsVolunteer}");
                _logger.LogInformation($"   - IsDonor: {user.IsDonor}");

                // Assign default "User" role
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation($"✅ User {model.Email} assigned to 'User' role.");
                    }
                    else
                    {
                        _logger.LogError($"❌ Failed to assign 'User' role to {model.Email}");
                        foreach (var error in roleResult.Errors)
                        {
                            _logger.LogError($"   - {error.Code}: {error.Description}");
                        }
                    }
                }
                else
                {
                    _logger.LogError("❌ 'User' role does not exist in the database!");
                }

                // Verify user was saved to database using UserService
                var savedUser = await _userService.GetUserByEmailAsync(model.Email);
                if (savedUser != null)
                {
                    _logger.LogInformation($"✅ Verified: User found in database with ID: {savedUser.Id}");
                    
                    // Add custom claims for the user
                    await AddUserClaimsAsync(savedUser);
                }
                else
                {
                    _logger.LogError($"❌ ERROR: User not found in database after creation!");
                }

                // Sign in the user automatically after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            // Log errors
            _logger.LogError($"❌ Failed to create user {model.Email}");
            foreach (var error in result.Errors)
            {
                _logger.LogError($"   - {error.Code}: {error.Description}");
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper action for two-factor authentication
        [HttpGet]
        public IActionResult LoginWith2fa(bool rememberMe)
        {
            // Implement two-factor authentication view if needed
            return View();
        }

        // Helper method to add custom claims
        private async Task AddUserClaimsAsync(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            
            // Remove old claims if they exist
            var firstNameClaim = existingClaims.FirstOrDefault(c => c.Type == "FirstName");
            if (firstNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, firstNameClaim);
            }
            
            var lastNameClaim = existingClaims.FirstOrDefault(c => c.Type == "LastName");
            if (lastNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, lastNameClaim);
            }

            // Add new claims
            await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
            await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
        }
    }
}
