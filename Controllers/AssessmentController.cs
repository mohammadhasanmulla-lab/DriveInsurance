using Microsoft.AspNetCore.Mvc;
using MyProject.Services;

namespace MyProject.Controllers
{
    /// <summary>
    /// Module 3 – Claim Assessment &amp; Surveyor Management
    /// </summary>
    public class AssessmentController : Controller
    {
        private readonly AssessmentService _assessmentService;
        private readonly ClaimService _claimService;

        public AssessmentController(AssessmentService assessmentService, ClaimService claimService)
        {
            _assessmentService = assessmentService;
            _claimService      = claimService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Surveyor")
            {
                int surveyorId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                return View(await _assessmentService.GetAssessmentsBySurveyorAsync(surveyorId));
            }
            return View(await _assessmentService.GetAllAssessmentsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AssignSurveyor(int claimId)
        {
            ViewBag.ClaimId   = claimId;
            ViewBag.Surveyors = await _assessmentService.GetAvailableSurveyorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSurveyor(int claimId, int surveyorId)
        {
            var (success, message, _) = await _assessmentService.AssignSurveyorAsync(claimId, surveyorId);
            if (!success)
            {
                TempData["Error"]     = message;
                ViewBag.ClaimId       = claimId;
                ViewBag.Surveyors     = await _assessmentService.GetAvailableSurveyorsAsync();
                return View();
            }
            TempData["Success"] = message;
            return RedirectToAction("GetAssessmentDetails", new { claimId });
        }

        [HttpGet]
        public async Task<IActionResult> RecordAssessment(int claimId)
        {
            var assessment = await _assessmentService.GetAssessmentByClaimAsync(claimId);
            if (assessment == null)
            {
                TempData["Error"] = "No assessment assigned for this claim yet.";
                return RedirectToAction("Index", "Claim");
            }
            return View(assessment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordAssessment(int assessmentId, int claimId, decimal assessedAmount, string remarks)
        {
            var (success, message) = await _assessmentService.RecordAssessmentAsync(assessmentId, assessedAmount, remarks);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("GetAssessmentDetails", new { claimId });
        }

        public async Task<IActionResult> GetAssessmentDetails(int claimId)
        {
            var assessment = await _assessmentService.GetAssessmentByClaimAsync(claimId);
            ViewBag.ClaimId   = claimId;
            ViewBag.Surveyors = await _assessmentService.GetAvailableSurveyorsAsync();
            return View(assessment);
        }
    }
}
