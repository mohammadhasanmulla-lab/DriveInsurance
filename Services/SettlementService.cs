using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Services
{
    /// <summary>
    /// Module 5 – Claim Settlement &amp; Payment Processing
    /// </summary>
    public class SettlementService
    {
        private readonly AppDbContext _db;

        public SettlementService(AppDbContext db) => _db = db;

        // ── Approve ───────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message, SettlementLogEntity? Settlement)>
            ApproveSettlementAsync(int claimId, string? remarks = null)
        {
            var claim = await _db.Claims
                                 .Include(c => c.Assessment)
                                 .Include(c => c.FraudCheck)
                                 .Include(c => c.Settlement)
                                 .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null) return (false, "Claim not found.", null);

            // Guard: fraud must be checked first
            if (claim.FraudCheck == null)
                return (false, "Fraud check must be completed before settlement.", null);

            if (claim.FraudCheck.RiskFlag == "HIGH")
                return (false, "Settlement cannot be approved — claim flagged HIGH risk by fraud engine.", null);

            // Settlement amount = assessed amount if available, else claimed amount
            decimal amount = claim.Assessment?.AssessedAmount > 0
                ? claim.Assessment.AssessedAmount
                : claim.ClaimAmount;

            var settlement = claim.Settlement ?? new SettlementLogEntity { ClaimId = claimId };
            settlement.SettlementAmount = amount;
            settlement.SettlementStatus = "APPROVED";
            settlement.Remarks          = remarks ?? "Approved after fraud and compliance review.";
            settlement.SettlementDate   = DateTime.Now;

            if (claim.Settlement == null) _db.SettlementLogs.Add(settlement);

            claim.ClaimStatus = "APPROVED";
            await _db.SaveChangesAsync();
            return (true, $"Settlement approved for amount {amount:C}.", settlement);
        }

        // ── Reject ────────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message)>
            RejectSettlementAsync(int claimId, string reason)
        {
            var claim = await _db.Claims
                                 .Include(c => c.Settlement)
                                 .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null) return (false, "Claim not found.");

            var settlement = claim.Settlement ?? new SettlementLogEntity { ClaimId = claimId };
            settlement.SettlementStatus = "REJECTED";
            settlement.Remarks          = reason;
            settlement.SettlementDate   = DateTime.Now;

            if (claim.Settlement == null) _db.SettlementLogs.Add(settlement);

            claim.ClaimStatus = "REJECTED";
            await _db.SaveChangesAsync();
            return (true, "Claim rejected successfully.");
        }

        // ── Process Payment ───────────────────────────────────────────────────

        public async Task<(bool Success, string Message, string? PaymentRef)>
            ProcessPaymentAsync(int claimId)
        {
            var settlement = await _db.SettlementLogs
                                      .FirstOrDefaultAsync(s => s.ClaimId == claimId);

            if (settlement == null)
                return (false, "No settlement record found for this claim.", null);

            if (settlement.SettlementStatus != "APPROVED")
                return (false, $"Settlement is not in APPROVED state (current: {settlement.SettlementStatus}).", null);

            // Simulate payment gateway call
            var paymentRef = $"PAY-{DateTime.Now:yyyyMMddHHmmss}-{claimId:D5}";
            settlement.SettlementStatus = "PAID";
            settlement.PaymentReference = paymentRef;
            settlement.Remarks          = $"Payment processed. Ref: {paymentRef}";
            await _db.SaveChangesAsync();

            return (true, $"Payment of {settlement.SettlementAmount:C} processed. Reference: {paymentRef}", paymentRef);
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public async Task<SettlementLogEntity?> GetSettlementByClaimAsync(int claimId)
            => await _db.SettlementLogs
                        .Include(s => s.Claim)
                        .FirstOrDefaultAsync(s => s.ClaimId == claimId);

        public async Task<List<SettlementLogEntity>> GetAllSettlementsAsync()
            => await _db.SettlementLogs
                        .Include(s => s.Claim)
                        .OrderByDescending(s => s.SettlementDate)
                        .ToListAsync();
    }
}
