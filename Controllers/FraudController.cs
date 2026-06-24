using Microsoft.AspNetCore.Mvc;
using MyProject.Services;

namespace MyProject.Controllers
{
    /// <summary>
    /// Module 4 – Fraud Detection &amp; Compliance Checks
    /// </summary>
    public class FraudController : Controller
    {
        private readonly FraudService _fraudService;

        public FraudController(FraudService fraudService)
        {
            _fraudService = fraudService;
        }

        // ── All fraud checks ──────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var checks = await _fraudService.GetAllFraudChecksAsync();
            return View(checks);
        }

        // ── Run fraud check ───────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> RunFraudCheck(int claimId)
        {
            var (success, message, fraudCheck) = await _fraudService.RunFraudCheckAsync(claimId);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("GetFraudScore", new { claimId });
        }

        // ── View fraud score for a claim ──────────────────────────────────────

        public async Task<IActionResult> GetFraudScore(int claimId)
        {
            var fraudCheck = await _fraudService.GetFraudCheckByClaimAsync(claimId);
            ViewBag.ClaimId = claimId;
            return View(fraudCheck);
        }
    }
}
