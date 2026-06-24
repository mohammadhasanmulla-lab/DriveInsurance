using Microsoft.AspNetCore.Mvc;
using MyProject.Services;

namespace MyProject.Controllers
{
    /// <summary>
    /// Module 5 – Claim Settlement &amp; Payment Processing
    /// </summary>
    public class SettlementController : Controller
    {
        private readonly SettlementService _settlementService;

        public SettlementController(SettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        // ── All settlements ───────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var settlements = await _settlementService.GetAllSettlementsAsync();
            return View(settlements);
        }

        // ── Approve ───────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ApproveSettlement(int claimId, string? remarks)
        {
            var (success, message, settlement) = await _settlementService.ApproveSettlementAsync(claimId, remarks);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("Details", new { claimId });
        }

        // ── Reject ────────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> RejectSettlement(int claimId, string reason)
        {
            var (success, message) = await _settlementService.RejectSettlementAsync(claimId, reason);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("Details", new { claimId });
        }

        // ── Process payment ───────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int claimId)
        {
            var (success, message, paymentRef) = await _settlementService.ProcessPaymentAsync(claimId);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("Details", new { claimId });
        }

        // ── Details ───────────────────────────────────────────────────────────

        public async Task<IActionResult> Details(int claimId)
        {
            var settlement = await _settlementService.GetSettlementByClaimAsync(claimId);
            ViewBag.ClaimId = claimId;
            return View(settlement);
        }
    }
}
