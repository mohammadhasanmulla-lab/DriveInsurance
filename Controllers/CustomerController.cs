using Microsoft.AspNetCore.Mvc;
using MyProject.Services;

namespace MyProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ClaimService _claimService;

        public CustomerController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Home");

            int customerId = int.Parse(userIdStr);
            var claims = await _claimService.GetClaimsByCustomerAsync(customerId);

            ViewBag.UserName    = HttpContext.Session.GetString("UserName") ?? "Customer";
            ViewBag.TotalClaims = claims.Count;
            ViewBag.Pending     = claims.Count(c => c.ClaimStatus is "REGISTERED" or "UNDER_REVIEW");
            ViewBag.Approved    = claims.Count(c => c.ClaimStatus == "APPROVED");
            ViewBag.Rejected    = claims.Count(c => c.ClaimStatus == "REJECTED");

            return View(claims);
        }
    }
}
