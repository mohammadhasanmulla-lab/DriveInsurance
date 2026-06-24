using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Services
{
    /// <summary>
    /// Module 1 – Claim Registration &amp; Policy Validation
    /// </summary>
    public class ClaimService
    {
        private readonly AppDbContext _db;

        // Mock policy store (mirrors Policies.json)
        private static readonly List<Policy> MockPolicies = new()
        {
            new Policy { Id = 1, PolicyName = "Auto Plus",    PolicyType = "Auto",   CoverageAmount = 100000m, PremiumAmount = 899m,  IsActive = true },
            new Policy { Id = 2, PolicyName = "Health Guard", PolicyType = "Health", CoverageAmount = 500000m, PremiumAmount = 1299m, IsActive = true },
            new Policy { Id = 3, PolicyName = "Home Shield",  PolicyType = "Home",   CoverageAmount = 250000m, PremiumAmount = 799m,  IsActive = true },
            new Policy { Id = 4, PolicyName = "Travel Safe",  PolicyType = "Travel", CoverageAmount = 50000m,  PremiumAmount = 299m,  IsActive = true },
        };

        public ClaimService(AppDbContext db) => _db = db;

        // ── Create ────────────────────────────────────────────────────────────

        public async Task<ClaimEntity> RegisterClaimAsync(ClaimEntity claim)
        {
            claim.ClaimStatus = "REGISTERED";
            claim.CreatedDate = DateTime.Now;
            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();
            return claim;
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public async Task<List<ClaimEntity>> GetAllClaimsAsync()
            => await _db.Claims
                        .Include(c => c.Documents)
                        .Include(c => c.Assessment)
                        .Include(c => c.FraudCheck)
                        .Include(c => c.Settlement)
                        .OrderByDescending(c => c.CreatedDate)
                        .ToListAsync();

        public async Task<List<ClaimEntity>> GetClaimsByCustomerAsync(int customerId)
            => await _db.Claims
                        .Include(c => c.Documents)
                        .Include(c => c.Assessment)
                        .Include(c => c.FraudCheck)
                        .Include(c => c.Settlement)
                        .Where(c => c.CustomerId == customerId)
                        .OrderByDescending(c => c.CreatedDate)
                        .ToListAsync();

        public async Task<ClaimEntity?> GetClaimByIdAsync(int id)
            => await _db.Claims
                        .Include(c => c.Documents)
                        .Include(c => c.Assessment)
                        .Include(c => c.FraudCheck)
                        .Include(c => c.Settlement)
                        .FirstOrDefaultAsync(c => c.ClaimId == id);

        // ── Update ────────────────────────────────────────────────────────────

        public async Task<bool> UpdateClaimAsync(ClaimEntity updated)
        {
            var existing = await _db.Claims.FindAsync(updated.ClaimId);
            if (existing == null) return false;

            existing.ClaimType   = updated.ClaimType;
            existing.ClaimAmount = updated.ClaimAmount;
            existing.Description = updated.Description;
            existing.ClaimStatus = updated.ClaimStatus;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string status)
        {
            var claim = await _db.Claims.FindAsync(claimId);
            if (claim == null) return false;
            claim.ClaimStatus = status;
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Policy Validation ─────────────────────────────────────────────────

        public (bool IsValid, string Message, Policy? Policy) ValidatePolicy(int policyId, decimal claimAmount)
        {
            var policy = MockPolicies.FirstOrDefault(p => p.Id == policyId);
            if (policy == null)
                return (false, "Policy not found.", null);
            if (!policy.IsActive)
                return (false, "Policy is no longer active.", null);
            if (claimAmount > policy.CoverageAmount)
                return (false, $"Claim amount exceeds coverage limit of {policy.CoverageAmount:C}.", policy);
            if (claimAmount <= 0)
                return (false, "Claim amount must be greater than zero.", policy);
            return (true, "Policy is valid.", policy);
        }

        public List<Policy> GetAvailablePolicies() => MockPolicies;
    }
}
