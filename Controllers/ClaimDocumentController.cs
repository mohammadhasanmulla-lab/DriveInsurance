using Microsoft.AspNetCore.Mvc;
using MyProject.Services;

namespace MyProject.Controllers
{
    /// <summary>
    /// Module 2 – Document Submission &amp; Verification
    /// </summary>
    public class ClaimDocumentController : Controller
    {
        private readonly ClaimDocumentService _docService;

        public ClaimDocumentController(ClaimDocumentService docService)
        {
            _docService = docService;
        }

        // ── List documents for a claim ────────────────────────────────────────

        public async Task<IActionResult> GetClaimDocuments(int claimId)
        {
            var docs = await _docService.GetDocumentsByClaimAsync(claimId);
            ViewBag.ClaimId = claimId;
            return View(docs);
        }

        // ── Upload ────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult UploadClaimDocument(int claimId)
        {
            ViewBag.ClaimId = claimId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadClaimDocument(int claimId, IFormFile file)
        {
            var (success, message, doc) = await _docService.UploadDocumentAsync(claimId, file);

            if (!success)
            {
                TempData["Error"] = message;
                ViewBag.ClaimId   = claimId;
                return View();
            }

            TempData["Success"] = message;
            return RedirectToAction("GetClaimDocuments", new { claimId });
        }

        // ── Verify (simulate OCR) ─────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ValidateClaimDocument(int documentId, int claimId)
        {
            var (success, message) = await _docService.ValidateDocumentAsync(documentId);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("GetClaimDocuments", new { claimId });
        }

        // ── Reject ────────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> RejectDocument(int documentId, int claimId, string reason)
        {
            await _docService.RejectDocumentAsync(documentId, reason);
            TempData["Success"] = "Document rejected.";
            return RedirectToAction("GetClaimDocuments", new { claimId });
        }
    }
}
