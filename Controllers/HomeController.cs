using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.Services;
using System.Diagnostics;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly ClaimService _claimService;

        public HomeController(UserService userService, ClaimService claimService)
        {
            _userService  = userService;
            _claimService = claimService;
        }

        // ── Public pages ──────────────────────────────────────────────────────

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        // ── Auth ──────────────────────────────────────────────────────────────

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return Redirect(UserService.GetRoleDashboardUrl(
                    HttpContext.Session.GetString("UserRole") ?? "Customer"));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message, user) = await _userService.LoginAsync(model.Email, model.Password);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View(model);
            }

            HttpContext.Session.SetString("UserId",         user!.Id.ToString());
            HttpContext.Session.SetString("UserEmail",      user.Email);
            HttpContext.Session.SetString("UserName",       user.FullName);
            HttpContext.Session.SetString("UserRole",       user.Role);
            HttpContext.Session.SetString("UserDepartment", user.Department);

            return Redirect(UserService.GetRoleDashboardUrl(user.Role));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message, user) = await _userService.RegisterAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["Success"] = "Account created successfully. Please log in.";
            return RedirectToAction("Login");
        }

        // ── Admin Dashboard ───────────────────────────────────────────────────

        public async Task<IActionResult> AdminDashboard()
        {
            var allUsers         = await _userService.GetAllUsersAsync();
            var claimsOfficers   = allUsers.Where(u => u.Role == "ClaimsOfficer").ToList();
            var surveyOfficers   = allUsers.Where(u => u.Role == "Surveyor").ToList();
            var complianceOfficers = allUsers.Where(u => u.Role == "ComplianceOfficer").ToList();

            // Build a legacy-compatible view model using real DB data
            var vm = new AdminDashboardViewModel
            {
                TotalUsers             = allUsers.Count,
                ClaimsOfficersCount    = claimsOfficers.Count,
                SurveyOfficersCount    = surveyOfficers.Count,
                ComplianceOfficersCount= complianceOfficers.Count,
                CustomersCount         = allUsers.Count(u => u.Role == "Customer"),
                // Map AppUser → legacy sub-classes for the existing view
                ClaimsOfficers   = claimsOfficers.Select(u => new ClaimsOfficer
                    { Id = u.Id, Username = u.Username, FullName = u.FullName, Email = u.Email,
                      Role = u.Role, CreatedDate = u.CreatedDate, IsActive = u.Status == "Active",
                      Department = u.Department }).ToList(),
                SurveyOfficers   = surveyOfficers.Select(u => new SurveyOfficer
                    { Id = u.Id, Username = u.Username, FullName = u.FullName, Email = u.Email,
                      Role = u.Role, CreatedDate = u.CreatedDate, IsActive = u.Status == "Active" }).ToList(),
                ComplianceOfficers = complianceOfficers.Select(u => new ComplianceOfficer
                    { Id = u.Id, Username = u.Username, FullName = u.FullName, Email = u.Email,
                      Role = u.Role, CreatedDate = u.CreatedDate, IsActive = u.Status == "Active" }).ToList(),
                AllUsers = allUsers.Select(u => new User
                    { Id = u.Id, Username = u.Username, FullName = u.FullName, Email = u.Email,
                      Role = u.Role, CreatedDate = u.CreatedDate, IsActive = u.Status == "Active" }).ToList(),
                Policies = _claimService.GetAvailablePolicies()
            };

            ViewBag.AllClaims = await _claimService.GetAllClaimsAsync();
            return View(vm);
        }

        public async Task<IActionResult> ClaimsOfficers()
        {
            var users = await _userService.GetUsersByRoleAsync("ClaimsOfficer");
            return View(users);
        }

        public async Task<IActionResult> SurveyOfficers()
        {
            var users = await _userService.GetUsersByRoleAsync("Surveyor");
            return View(users);
        }

        public async Task<IActionResult> ComplianceOfficers()
        {
            var users = await _userService.GetUsersByRoleAsync("ComplianceOfficer");
            return View(users);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            await _userService.ToggleUserStatusAsync(id);
            TempData["Success"] = "User status updated.";
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction("ManageUsers");
        }

        public IActionResult ManagePolicies()
        {
            var policies = _claimService.GetAvailablePolicies();
            return View(policies);
        }

        public IActionResult AddPolicy() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPolicy(Policy policy)
        {
            TempData["Success"] = "Policy saved successfully.";
            return RedirectToAction("ManagePolicies");
        }
    }
}
