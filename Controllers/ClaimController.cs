using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.Services;

namespace MyProject.Controllers
{
    /// <summary>
    /// Module 1 – Claim Registration &amp; Policy Validation
    /// </summary>
    public class ClaimController : Controller
    {
        private readonly ClaimService _claimService;

        public ClaimController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        // ── List ──────────────────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            List<ClaimEntity> claims;

            if (role == "Customer")
            {
                int customerId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                claims = await _claimService.GetClaimsByCustomerAsync(customerId);
            }
            else
            {
                claims = await _claimService.GetAllClaimsAsync();
            }

            return View(claims);
        }

        // ── Register ──────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult RegisterClaim()
        {
            ViewBag.Policies = _claimService.GetAvailablePolicies();
            return View(new ClaimEntity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterClaim(ClaimEntity model)
        {
            // Validate the policy before saving
            var (isValid, message, policy) = _claimService.ValidatePolicy(model.PolicyId, model.ClaimAmount);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, message);
                ViewBag.Policies = _claimService.GetAvailablePolicies();
                return View(model);
            }

            // Attach current customer from session
            model.CustomerId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            if (!ModelState.IsValid)
            {
                ViewBag.Policies = _claimService.GetAvailablePolicies();
                return View(model);
            }

            var claim = await _claimService.RegisterClaimAsync(model);
            TempData["Success"] = $"Claim #{claim.ClaimId} registered successfully.";
            return RedirectToAction("GetClaimDetails", new { id = claim.ClaimId });
        }

        // ── Details ───────────────────────────────────────────────────────────

        public async Task<IActionResult> GetClaimDetails(int id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> UpdateClaim(int id)
        {
            var claim = await _claimService.GetClaimByIdAsync(id);
            if (claim == null) return NotFound();
            ViewBag.Policies = _claimService.GetAvailablePolicies();
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClaim(ClaimEntity model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Policies = _claimService.GetAvailablePolicies();
                return View(model);
            }

            var success = await _claimService.UpdateClaimAsync(model);
            if (!success) return NotFound();

            TempData["Success"] = "Claim updated successfully.";
            return RedirectToAction("GetClaimDetails", new { id = model.ClaimId });
        }

        // ── Policy Validation (AJAX-friendly endpoint) ────────────────────────

        [HttpGet]
        public IActionResult ValidatePolicy(int policyId, decimal amount)
        {
            var (isValid, message, policy) = _claimService.ValidatePolicy(policyId, amount);
            return Json(new { isValid, message, policyName = policy?.PolicyName, coverage = policy?.CoverageAmount });
        }

        // ── Status Update (for officers/admin) ────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int claimId, string status)
        {
            var validStatuses = new[] { "REGISTERED", "UNDER_REVIEW", "APPROVED", "REJECTED" };
            if (!validStatuses.Contains(status))
                return BadRequest("Invalid status value.");

            await _claimService.UpdateClaimStatusAsync(claimId, status);
            TempData["Success"] = $"Claim status updated to {status}.";
            return RedirectToAction("GetClaimDetails", new { id = claimId });
        }
    }
}
