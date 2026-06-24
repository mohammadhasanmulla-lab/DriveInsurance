using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Services
{
    /// <summary>
    /// Module 2 – Document Submission &amp; Verification (simulated OCR checks)
    /// </summary>
    public class ClaimDocumentService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ClaimDocumentService(AppDbContext db, IWebHostEnvironment env)
        {
            _db  = db;
            _env = env;
        }

        // ── Upload ────────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message, ClaimDocumentEntity? Doc)>
            UploadDocumentAsync(int claimId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "No file provided.", null);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return (false, $"File type '{ext}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}", null);

            if (file.Length > MaxFileSizeBytes)
                return (false, "File size exceeds the 5 MB limit.", null);

            // Save to wwwroot/uploads/claims/<claimId>/
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "claims", claimId.ToString());
            Directory.CreateDirectory(uploadDir);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath   = Path.Combine(uploadDir, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var relativePath = $"/uploads/claims/{claimId}/{uniqueName}";

            var doc = new ClaimDocumentEntity
            {
                ClaimId            = claimId,
                DocumentName       = file.FileName,
                FilePath           = relativePath,
                VerificationStatus = "PENDING",
                UploadedDate       = DateTime.Now
            };

            _db.ClaimDocuments.Add(doc);
            await _db.SaveChangesAsync();
            return (true, "Document uploaded successfully.", doc);
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public async Task<List<ClaimDocumentEntity>> GetDocumentsByClaimAsync(int claimId)
            => await _db.ClaimDocuments
                        .Where(d => d.ClaimId == claimId)
                        .OrderByDescending(d => d.UploadedDate)
                        .ToListAsync();

        public async Task<ClaimDocumentEntity?> GetDocumentByIdAsync(int documentId)
            => await _db.ClaimDocuments.FindAsync(documentId);

        // ── Simulated OCR Verification ────────────────────────────────────────

        public async Task<(bool Success, string Message)>
            ValidateDocumentAsync(int documentId)
        {
            var doc = await _db.ClaimDocuments.FindAsync(documentId);
            if (doc == null) return (false, "Document not found.");

            // Simulated OCR: images/PDFs pass; anything else flagged
            var ext = Path.GetExtension(doc.DocumentName).ToLowerInvariant();
            bool ocrPass = ext is ".pdf" or ".jpg" or ".jpeg" or ".png";

            if (ocrPass)
            {
                doc.VerificationStatus = "VERIFIED";
                await _db.SaveChangesAsync();
                return (true, "Document verified successfully via OCR simulation.");
            }
            else
            {
                doc.VerificationStatus = "REJECTED";
                doc.RejectionReason    = "OCR simulation could not extract valid content from this file type.";
                await _db.SaveChangesAsync();
                return (false, "Document failed OCR verification.");
            }
        }

        public async Task<bool> RejectDocumentAsync(int documentId, string reason)
        {
            var doc = await _db.ClaimDocuments.FindAsync(documentId);
            if (doc == null) return false;
            doc.VerificationStatus = "REJECTED";
            doc.RejectionReason    = reason;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
