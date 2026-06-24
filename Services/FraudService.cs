using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Services
{
    /// <summary>
    /// Module 4 – Fraud Detection &amp; Compliance Checks (rule-based scoring engine)
    /// </summary>
    public class FraudService
    {
        private readonly AppDbContext _db;

        public FraudService(AppDbContext db) => _db = db;

        // ── Rule-based Scoring ────────────────────────────────────────────────
        // Score 0–100; each rule adds points.
        //   0–30  → LOW
        //  31–60  → MEDIUM
        //  61–100 → HIGH

        public async Task<(bool Success, string Message, FraudCheckEntity? FraudCheck)>
            RunFraudCheckAsync(int claimId)
        {
            var claim = await _db.Claims
                                 .Include(c => c.Documents)
                                 .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null) return (false, "Claim not found.", null);

            int score = 0;
            var notes = new List<string>();

            // Rule 1 – High claim amount relative to policy coverage
            if (claim.ClaimAmount > 80_000m)  { score += 25; notes.Add("High claim amount (>80,000)."); }
            else if (claim.ClaimAmount > 40_000m) { score += 10; notes.Add("Elevated claim amount (>40,000)."); }

            // Rule 2 – Claim filed very quickly (within 1 day of creation — simulated)
            if ((DateTime.Now - claim.CreatedDate).TotalHours < 2)
            { score += 20; notes.Add("Claim filed within 2 hours of policy creation."); }

            // Rule 3 – No documents submitted
            if (!claim.Documents.Any())
            { score += 30; notes.Add("No supporting documents attached."); }
            else if (claim.Documents.All(d => d.VerificationStatus == "REJECTED"))
            { score += 20; notes.Add("All submitted documents failed verification."); }
            else if (claim.Documents.Any(d => d.VerificationStatus == "PENDING"))
            { score += 10; notes.Add("Some documents are still pending verification."); }

            // Rule 4 – Duplicate claim type for the same customer in the past 30 days
            var recent = await _db.Claims
                .Where(c => c.CustomerId == claim.CustomerId
                         && c.ClaimType == claim.ClaimType
                         && c.ClaimId   != claim.ClaimId
                         && c.CreatedDate >= DateTime.Now.AddDays(-30))
                .CountAsync();
            if (recent > 0) { score += 15; notes.Add($"Duplicate claim type filed {recent} time(s) in last 30 days."); }

            // Cap at 100
            score = Math.Min(score, 100);

            string riskFlag = score switch
            {
                <= 30 => "LOW",
                <= 60 => "MEDIUM",
                _     => "HIGH"
            };

            // Remove existing check if re-running
            var existing = await _db.FraudChecks.FirstOrDefaultAsync(f => f.ClaimId == claimId);
            if (existing != null) _db.FraudChecks.Remove(existing);

            var fraudCheck = new FraudCheckEntity
            {
                ClaimId   = claimId,
                FraudScore = score,
                RiskFlag  = riskFlag,
                CheckDate = DateTime.Now,
                Notes     = notes.Any() ? string.Join(" | ", notes) : "No risk indicators detected."
            };

            _db.FraudChecks.Add(fraudCheck);

            // If HIGH, push claim to UNDER_REVIEW
            if (riskFlag == "HIGH")
            {
                claim.ClaimStatus = "UNDER_REVIEW";
            }

            await _db.SaveChangesAsync();
            return (true, $"Fraud check completed. Risk: {riskFlag} (Score: {score}).", fraudCheck);
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public async Task<FraudCheckEntity?> GetFraudCheckByClaimAsync(int claimId)
            => await _db.FraudChecks
                        .Include(f => f.Claim)
                        .FirstOrDefaultAsync(f => f.ClaimId == claimId);

        public async Task<List<FraudCheckEntity>> GetAllFraudChecksAsync()
            => await _db.FraudChecks
                        .Include(f => f.Claim)
                        .OrderByDescending(f => f.CheckDate)
                        .ToListAsync();
    }
}
