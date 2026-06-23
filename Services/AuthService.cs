using MyProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class AuthService
    {
        private static readonly List<LoginCredential> MockCredentials = new()
        {
            // Admin
            new LoginCredential 
            { 
                Id = 1, Email = "admin@company.com", Username = "admin_user", Password = "Admin@123", 
                Role = "Admin", FullName = "Admin User", Department = "Administration", Status = "Active"
            },
            // Claims Officers
            new LoginCredential 
            { 
                Id = 2, Email = "jane@company.com", Username = "jane_smith", Password = "Jane@123", 
                Role = "ClaimsOfficer", FullName = "Jane Smith", Department = "Auto Claims", Status = "Active"
            },
            new LoginCredential 
            { 
                Id = 7, Email = "emily@company.com", Username = "emily_davis", Password = "Emily@123", 
                Role = "ClaimsOfficer", FullName = "Emily Davis", Department = "Health Claims", Status = "Active"
            },
            // Survey Officers
            new LoginCredential 
            { 
                Id = 3, Email = "mike@company.com", Username = "mike_wilson", Password = "Mike@123", 
                Role = "Surveyor", FullName = "Mike Wilson", Department = "Survey Department", Status = "Active"
            },
            new LoginCredential 
            { 
                Id = 8, Email = "david@company.com", Username = "david_miller", Password = "David@123", 
                Role = "Surveyor", FullName = "David Miller", Department = "Survey Department", Status = "Active"
            },
            // Compliance Officer
            new LoginCredential 
            { 
                Id = 4, Email = "sara@company.com", Username = "sara_johnson", Password = "Sara@123", 
                Role = "ComplianceOfficer", FullName = "Sara Johnson", Department = "Compliance", Status = "Active"
            },
            // Customer
            new LoginCredential 
            { 
                Id = 5, Email = "john@company.com", Username = "john_doe", Password = "John@123", 
                Role = "Customer", FullName = "John Doe", Department = "N/A", Status = "Active"
            }
        };

        /// <summary>
        /// Authenticate user with email and password
        /// </summary>
        public static AuthenticatedUser Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var credential = MockCredentials.FirstOrDefault(c => 
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                c.Password == password);

            if (credential == null)
            {
                return null;
            }

            return new AuthenticatedUser
            {
                Id = credential.Id,
                Email = credential.Email,
                Username = credential.Username,
                FullName = credential.FullName,
                Role = credential.Role,
                Department = credential.Department,
                LoginTime = DateTime.Now
            };
        }

        /// <summary>
        /// Get all mock credentials (for reference/testing)
        /// </summary>
        public static List<LoginCredential> GetAllCredentials()
        {
            return MockCredentials;
        }

        /// <summary>
        /// Get credential by email
        /// </summary>
        public static LoginCredential GetCredentialByEmail(string email)
        {
            return MockCredentials.FirstOrDefault(c => 
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Validate if email exists
        /// </summary>
        public static bool EmailExists(string email)
        {
            return MockCredentials.Any(c => 
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get users by role
        /// </summary>
        public static List<LoginCredential> GetUsersByRole(string role)
        {
            return MockCredentials.Where(c => 
                c.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Get user dashboard URL based on role
        /// </summary>
        public static string GetRoleDashboardUrl(string role)
        {
            // Map roles to the existing controller actions in the app.
            // The project currently implements actions named *Officers (e.g. ClaimsOfficers)
            // so redirect there rather than to non-existent *Dashboard routes.
            return role switch
            {
                "Admin" => "/Home/AdminDashboard",
                "ClaimsOfficer" => "/Home/ClaimsOfficers",
                "Surveyor" => "/Home/SurveyOfficers",
                "ComplianceOfficer" => "/Home/ComplianceOfficers",
                // Customers currently have no dedicated dashboard; send to home index
                "Customer" => "/Home/Index",
                _ => "/Home/Index"
            };
        }
    }
}
