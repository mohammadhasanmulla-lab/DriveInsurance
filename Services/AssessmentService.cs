using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Services
{
    /// <summary>
    /// Module 3 – Claim Assessment &amp; Surveyor Management
    /// </summary>
    public class AssessmentService
    {
        private readonly AppDbContext _db;

        public AssessmentService(AppDbContext db) => _db = db;

        // ── Assign ────────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message, AssessmentEntity? Assessment)>
            AssignSurveyorAsync(int claimId, int surveyorId)
        {
            var surveyor = await _db.Users.FirstOrDefaultAsync(u => u.Id == surveyorId && u.Role == "Surveyor");
            if (surveyor == null)
                return (false, "Surveyor not found.", null);

            var existing = await _db.Assessments.FirstOrDefaultAsync(a => a.ClaimId == claimId);
            if (existing != null)
                return (false, "An assessment is already assigned for this claim.", null);

            var assessment = new AssessmentEntity
            {
                ClaimId          = claimId,
                SurveyorId       = surveyorId,
                AssessmentStatus = "PENDING",
                AssessmentDate   = DateTime.Now
            };

            _db.Assessments.Add(assessment);
            await _db.SaveChangesAsync();
            return (true, $"Surveyor {surveyor.FullName} assigned successfully.", assessment);        }

        // ── Record ────────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message)>
            RecordAssessmentAsync(int assessmentId, decimal assessedAmount, string remarks)
        {
            var assessment = await _db.Assessments.FindAsync(assessmentId);
            if (assessment == null) return (false, "Assessment not found.");

            assessment.AssessedAmount     = assessedAmount;
            assessment.AssessmentRemarks  = remarks;
            assessment.AssessmentStatus   = "COMPLETED";
            assessment.AssessmentDate     = DateTime.Now;
            await _db.SaveChangesAsync();
            return (true, "Assessment recorded successfully.");
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public async Task<AssessmentEntity?> GetAssessmentByClaimAsync(int claimId)
            => await _db.Assessments
                        .Include(a => a.Claim)
                        .FirstOrDefaultAsync(a => a.ClaimId == claimId);

        public async Task<AssessmentEntity?> GetAssessmentByIdAsync(int id)
            => await _db.Assessments.Include(a => a.Claim).FirstOrDefaultAsync(a => a.AssessmentId == id);

        public async Task<List<AssessmentEntity>> GetAssessmentsBySurveyorAsync(int surveyorId)
            => await _db.Assessments
                        .Include(a => a.Claim)
                        .Where(a => a.SurveyorId == surveyorId)
                        .OrderByDescending(a => a.AssessmentDate)
                        .ToListAsync();

        public async Task<List<AssessmentEntity>> GetAllAssessmentsAsync()
            => await _db.Assessments
                        .Include(a => a.Claim)
                        .OrderByDescending(a => a.AssessmentDate)
                        .ToListAsync();

        public async Task<List<AppUser>> GetAvailableSurveyorsAsync()
            => await _db.Users
                        .Where(u => u.Role == "Surveyor" && u.Status == "Active")
                        .OrderBy(u => u.FullName)
                        .ToListAsync();
    }
}
