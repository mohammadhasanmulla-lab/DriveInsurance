using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;
using BCrypt.Net;

namespace MyProject.Services
{
    /// <summary>
    /// DB-backed authentication and registration service.
    /// Passwords are hashed with BCrypt (work factor 12).
    /// </summary>
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db) => _db = db;

        // ── Register ──────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message, AppUser? User)>
            RegisterAsync(RegisterViewModel model)
        {
            // Duplicate email check
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
                return (false, "An account with this email already exists.", null);

            // Duplicate username check
            if (await _db.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
                return (false, "This username is already taken.", null);

            var user = new AppUser
            {
                FullName     = model.FullName.Trim(),
                Username     = model.Username.Trim(),
                Email        = model.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, workFactor: 12),
                Role         = model.Role,
                Department   = GetDefaultDepartment(model.Role),
                Status       = "Active",
                CreatedDate  = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return (true, "Account created successfully.", user);
        }

        // ── Login ─────────────────────────────────────────────────────────────

        public async Task<(bool Success, string Message, AppUser? User)>
            LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Please enter both email and password.", null);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

            if (user == null)
                return (false, "Invalid email or password.", null);

            if (user.Status == "Inactive")
                return (false, "Your account has been deactivated. Contact support.", null);

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, "Invalid email or password.", null);

            return (true, "Login successful.", user);
        }

        // ── Admin helpers ─────────────────────────────────────────────────────

        public async Task<List<AppUser>> GetAllUsersAsync()
            => await _db.Users.OrderByDescending(u => u.CreatedDate).ToListAsync();

        public async Task<List<AppUser>> GetUsersByRoleAsync(string role)
            => await _db.Users.Where(u => u.Role == role).OrderBy(u => u.FullName).ToListAsync();

        public async Task<AppUser?> GetUserByIdAsync(int id)
            => await _db.Users.FindAsync(id);

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            user.Status = user.Status == "Active" ? "Inactive" : "Active";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Role dashboard routing ─────────────────────────────────────────────

        public static string GetRoleDashboardUrl(string role) => role switch
        {
            "Admin"             => "/Home/AdminDashboard",
            "ClaimsOfficer"     => "/Home/ClaimsOfficers",
            "Surveyor"          => "/Assessment/Index",
            "ComplianceOfficer" => "/Fraud/Index",
            "Customer"          => "/Customer/Dashboard",
            _                   => "/Home/Index"
        };

        // ── Seeding ───────────────────────────────────────────────────────────

        /// <summary>
        /// Seeds one account per role if that role has no users yet.
        /// Runs once at startup — safe to call on every launch.
        /// </summary>
        public static async Task SeedAdminAsync(AppDbContext db)
        {
            // Seed data: (FullName, Username, Email, PlainPassword, Role, Department)
            var seeds = new[]
            {
                ("Admin User",          "admin",        "admin@drivesure.com",      "Admin@123",      "Admin",             "Administration"),
                ("Jane Smith",          "jane_smith",   "jane@drivesure.com",       "Claims@123",     "ClaimsOfficer",     "Claims Department"),
                ("Mike Wilson",         "mike_wilson",  "mike@drivesure.com",       "Survey@123",     "Surveyor",          "Survey Department"),
                ("Sara Johnson",        "sara_johnson", "sara@drivesure.com",       "Comply@123",     "ComplianceOfficer", "Compliance Department"),
                ("John Doe",            "john_doe",     "john@drivesure.com",       "Customer@123",   "Customer",          "N/A"),
            };

            foreach (var (fullName, username, email, password, role, dept) in seeds)
            {
                // Only insert if this exact email doesn't exist yet
                if (!await db.Users.AnyAsync(u => u.Email == email))
                {
                    db.Users.Add(new AppUser
                    {
                        FullName     = fullName,
                        Username     = username,
                        Email        = email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
                        Role         = role,
                        Department   = dept,
                        Status       = "Active",
                        CreatedDate  = DateTime.Now
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string GetDefaultDepartment(string role) => role switch
        {
            "ClaimsOfficer"     => "Claims Department",
            "Surveyor"          => "Survey Department",
            "ComplianceOfficer" => "Compliance Department",
            "Admin"             => "Administration",
            _                   => "N/A"
        };
    }
}
