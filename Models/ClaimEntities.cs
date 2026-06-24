using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models
{
    // ─── Module 1: Claim Registration & Policy Validation ───────────────────────

    public class ClaimEntity
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public int PolicyId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required, MaxLength(50)]
        public string ClaimType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(15,2)")]
        public decimal ClaimAmount { get; set; }

        [Required, MaxLength(20)]
        public string ClaimStatus { get; set; } = "REGISTERED"; // REGISTERED | UNDER_REVIEW | APPROVED | REJECTED

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation
        public ICollection<ClaimDocumentEntity> Documents { get; set; } = new List<ClaimDocumentEntity>();
        public AssessmentEntity? Assessment { get; set; }
        public FraudCheckEntity? FraudCheck { get; set; }
        public SettlementLogEntity? Settlement { get; set; }
    }

    // ─── Module 2: Document Submission & Verification ───────────────────────────

    public class ClaimDocumentEntity
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [Required, MaxLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string VerificationStatus { get; set; } = "PENDING"; // PENDING | VERIFIED | REJECTED

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? RejectionReason { get; set; }

        // Navigation
        public ClaimEntity? Claim { get; set; }
    }

    // ─── Module 3: Claim Assessment & Surveyor Management ───────────────────────

    public class AssessmentEntity
    {
        [Key]
        public int AssessmentId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [Required]
        public int SurveyorId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal AssessedAmount { get; set; }

        [MaxLength(255)]
        public string? AssessmentRemarks { get; set; }

        [MaxLength(20)]
        public string AssessmentStatus { get; set; } = "PENDING"; // PENDING | COMPLETED

        public DateTime AssessmentDate { get; set; } = DateTime.Now;

        // Navigation
        public ClaimEntity? Claim { get; set; }
    }

    // ─── Module 4: Fraud Detection & Compliance Checks ──────────────────────────

    public class FraudCheckEntity
    {
        [Key]
        public int FraudId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        public int FraudScore { get; set; }  // 0–100

        [Required, MaxLength(10)]
        public string RiskFlag { get; set; } = "LOW"; // LOW | MEDIUM | HIGH

        public DateTime CheckDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public ClaimEntity? Claim { get; set; }
    }

    // ─── Module 5: Claim Settlement & Payment Processing ────────────────────────

    public class SettlementLogEntity
    {
        [Key]
        public int SettlementId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal SettlementAmount { get; set; }

        [MaxLength(50)]
        public string SettlementStatus { get; set; } = "PENDING"; // PENDING | APPROVED | REJECTED | PAID

        [MaxLength(255)]
        public string? Remarks { get; set; }

        public DateTime SettlementDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? PaymentReference { get; set; }

        // Navigation
        public ClaimEntity? Claim { get; set; }
    }
}
