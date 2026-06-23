using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.Services;
using System.Diagnostics;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email) || string.IsNullOrWhiteSpace(model?.Password))
            {
                ViewBag.ErrorMessage = "Please enter both email and password.";
                return View();
            }

            // Authenticate user
            var authenticatedUser = AuthService.Authenticate(model.Email, model.Password);

            if (authenticatedUser == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
                return View();
            }

            // Store user in session
            HttpContext.Session.SetString("UserId", authenticatedUser.Id.ToString());
            HttpContext.Session.SetString("UserEmail", authenticatedUser.Email);
            HttpContext.Session.SetString("UserName", authenticatedUser.FullName);
            HttpContext.Session.SetString("UserRole", authenticatedUser.Role);
            HttpContext.Session.SetString("UserDepartment", authenticatedUser.Department);

            // Redirect based on role
            string redirectUrl = AuthService.GetRoleDashboardUrl(authenticatedUser.Role);
            ViewBag.SuccessMessage = $"Welcome {authenticatedUser.FullName}! You are logged in as {authenticatedUser.Role}.";

            return Redirect(redirectUrl);
        }

        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            ViewBag.SuccessMessage = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // =============== ADMIN METHODS ===============

        public IActionResult AdminDashboard()
        {
            var claimsOfficers = GetMockClaimsOfficers();
            var surveyOfficers = GetMockSurveyOfficers();
            var complianceOfficers = GetMockComplianceOfficers();
            var allUsers = GetMockAllUsers();
            var policies = GetMockPolicies();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = allUsers.Count,
                ClaimsOfficersCount = claimsOfficers.Count,
                SurveyOfficersCount = surveyOfficers.Count,
                ComplianceOfficersCount = complianceOfficers.Count,
                CustomersCount = allUsers.Count(u => u.Role == "Customer"),
                ClaimsOfficers = claimsOfficers,
                SurveyOfficers = surveyOfficers,
                ComplianceOfficers = complianceOfficers,
                AllUsers = allUsers,
                Policies = policies
            };

            return View(viewModel);
        }

        public IActionResult ClaimsOfficers()
        {
            var claimsOfficers = GetMockClaimsOfficers();
            return View(claimsOfficers);
        }

        public IActionResult SurveyOfficers()
        {
            var surveyOfficers = GetMockSurveyOfficers();
            return View(surveyOfficers);
        }

        public IActionResult ComplianceOfficers()
        {
            var complianceOfficers = GetMockComplianceOfficers();
            return View(complianceOfficers);
        }

        public IActionResult ManageUsers()
        {
            var users = GetMockAllUsers();
            return View(users);
        }

        public IActionResult ManagePolicies()
        {
            var policies = GetMockPolicies();
            return View(policies);
        }

        public IActionResult AddPolicy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddPolicy(Policy policy)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Policy added successfully!";
            }
            return RedirectToAction("ManagePolicies");
        }

        public IActionResult DeleteUser(int id)
        {
            TempData["SuccessMessage"] = $"User with ID {id} has been deleted successfully!";
            return RedirectToAction("ManageUsers");
        }

        // =============== MOCK DATA METHODS ===============

        private static List<ClaimsOfficer> GetMockClaimsOfficers()
        {
            return new List<ClaimsOfficer>
            {
                new ClaimsOfficer 
                { 
                    Id = 2, Username = "jane_smith", Email = "jane@company.com", Role = "ClaimsOfficer", 
                    CreatedDate = DateTime.Now.AddDays(-45), IsActive = true,
                    Department = "Auto Claims", ClaimsHandled = 156, ApprovalRate = 92.5m
                },
                new ClaimsOfficer 
                { 
                    Id = 7, Username = "emily_davis", Email = "emily@company.com", Role = "ClaimsOfficer", 
                    CreatedDate = DateTime.Now.AddDays(-35), IsActive = true,
                    Department = "Health Claims", ClaimsHandled = 203, ApprovalRate = 88.3m
                }
            };
        }

        private static List<SurveyOfficer> GetMockSurveyOfficers()
        {
            return new List<SurveyOfficer>
            {
                new SurveyOfficer 
                { 
                    Id = 3, Username = "mike_wilson", Email = "mike@company.com", Role = "Surveyor", 
                    CreatedDate = DateTime.Now.AddDays(-60), IsActive = true,
                    Specialization = "Auto Damage", SurveysCompleted = 89, AccuracyRate = 96.2m
                },
                new SurveyOfficer 
                { 
                    Id = 8, Username = "david_miller", Email = "david@company.com", Role = "Surveyor", 
                    CreatedDate = DateTime.Now.AddDays(-25), IsActive = true,
                    Specialization = "Property Damage", SurveysCompleted = 45, AccuracyRate = 94.8m
                }
            };
        }

        private static List<ComplianceOfficer> GetMockComplianceOfficers()
        {
            return new List<ComplianceOfficer>
            {
                new ComplianceOfficer 
                { 
                    Id = 4, Username = "sara_johnson", Email = "sara@company.com", Role = "ComplianceOfficer", 
                    CreatedDate = DateTime.Now.AddDays(-15), IsActive = true,
                    CertificationLevel = "Level 3", AuditsCompleted = 23, ComplianceStatus = "Compliant"
                }
            };
        }

        private static List<User> GetMockAllUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Username = "john_doe", Email = "john@company.com", Role = "Customer", CreatedDate = DateTime.Now.AddDays(-30), IsActive = true },
                new User { Id = 2, Username = "jane_smith", Email = "jane@company.com", Role = "ClaimsOfficer", CreatedDate = DateTime.Now.AddDays(-45), IsActive = true },
                new User { Id = 3, Username = "mike_wilson", Email = "mike@company.com", Role = "Surveyor", CreatedDate = DateTime.Now.AddDays(-60), IsActive = true },
                new User { Id = 4, Username = "sara_johnson", Email = "sara@company.com", Role = "ComplianceOfficer", CreatedDate = DateTime.Now.AddDays(-15), IsActive = true },
                new User { Id = 5, Username = "admin_user", Email = "admin@company.com", Role = "Admin", CreatedDate = DateTime.Now.AddDays(-90), IsActive = true },
                new User { Id = 6, Username = "robert_brown", Email = "robert@company.com", Role = "Customer", CreatedDate = DateTime.Now.AddDays(-20), IsActive = false },
                new User { Id = 7, Username = "emily_davis", Email = "emily@company.com", Role = "ClaimsOfficer", CreatedDate = DateTime.Now.AddDays(-35), IsActive = true },
                new User { Id = 8, Username = "david_miller", Email = "david@company.com", Role = "Surveyor", CreatedDate = DateTime.Now.AddDays(-25), IsActive = true }
            };
        }

        private static List<Policy> GetMockPolicies()
        {
            return new List<Policy>
            {
                new Policy { Id = 1, PolicyName = "Auto Plus", Description = "Comprehensive auto insurance", CoverageAmount = 100000m, PremiumAmount = 899m, PolicyType = "Auto", CreatedDate = DateTime.Now.AddMonths(-6), IsActive = true },
                new Policy { Id = 2, PolicyName = "Health Guard", Description = "Full health coverage", CoverageAmount = 500000m, PremiumAmount = 1299m, PolicyType = "Health", CreatedDate = DateTime.Now.AddMonths(-4), IsActive = true },
                new Policy { Id = 3, PolicyName = "Home Shield", Description = "Complete home protection", CoverageAmount = 250000m, PremiumAmount = 799m, PolicyType = "Home", CreatedDate = DateTime.Now.AddMonths(-2), IsActive = true },
                new Policy { Id = 4, PolicyName = "Travel Safe", Description = "International travel coverage", CoverageAmount = 50000m, PremiumAmount = 299m, PolicyType = "Travel", CreatedDate = DateTime.Now.AddMonths(-3), IsActive = true }
            };
        }
    }
}
