using Microsoft.EntityFrameworkCore;
using MyProject.Models;

namespace MyProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── DbSets ────────────────────────────────────────────────────────────
        public DbSet<AppUser> Users { get; set; }
        public DbSet<ClaimEntity> Claims { get; set; }
        public DbSet<ClaimDocumentEntity> ClaimDocuments { get; set; }
        public DbSet<AssessmentEntity> Assessments { get; set; }
        public DbSet<FraudCheckEntity> FraudChecks { get; set; }
        public DbSet<SettlementLogEntity> SettlementLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Users ─────────────────────────────────────────────────────────
            modelBuilder.Entity<AppUser>(e =>
            {
                e.ToTable("Users");
                e.HasIndex(u => u.Email).IsUnique();
                e.HasIndex(u => u.Username).IsUnique();
                e.Property(u => u.Status).HasDefaultValue("Active");
                e.Property(u => u.Role).HasDefaultValue("Customer");
            });

            // ── Claim ─────────────────────────────────────────────────────────
            modelBuilder.Entity<ClaimEntity>(e =>
            {
                e.ToTable("Claims");
                e.Property(c => c.ClaimStatus).HasDefaultValue("REGISTERED");
                e.HasMany(c => c.Documents)
                    .WithOne(d => d.Claim)
                    .HasForeignKey(d => d.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.Assessment)
                    .WithOne(a => a.Claim)
                    .HasForeignKey<AssessmentEntity>(a => a.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.FraudCheck)
                    .WithOne(f => f.Claim)
                    .HasForeignKey<FraudCheckEntity>(f => f.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.Settlement)
                    .WithOne(s => s.Claim)
                    .HasForeignKey<SettlementLogEntity>(s => s.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── ClaimDocument ─────────────────────────────────────────────────
            modelBuilder.Entity<ClaimDocumentEntity>(e =>
            {
                e.ToTable("ClaimDocuments");
                e.Property(d => d.VerificationStatus).HasDefaultValue("PENDING");
            });

            // ── Assessment ────────────────────────────────────────────────────
            modelBuilder.Entity<AssessmentEntity>(e =>
            {
                e.ToTable("Assessments");
                e.Property(a => a.AssessmentStatus).HasDefaultValue("PENDING");
            });

            // ── FraudCheck ────────────────────────────────────────────────────
            modelBuilder.Entity<FraudCheckEntity>(e =>
            {
                e.ToTable("FraudChecks");
                e.Property(f => f.RiskFlag).HasDefaultValue("LOW");
            });

            // ── SettlementLog ─────────────────────────────────────────────────
            modelBuilder.Entity<SettlementLogEntity>(e =>
            {
                e.ToTable("SettlementLogs");
                e.Property(s => s.SettlementStatus).HasDefaultValue("PENDING");
            });
        }
    }
}
