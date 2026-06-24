using System;
using System.Collections.Generic;

namespace MyProject.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClaimsOfficer : User
    {
        public string Department { get; set; }
        public int ClaimsHandled { get; set; }
        public decimal ApprovalRate { get; set; }
    }

    public class SurveyOfficer : User
    {
        public string Specialization { get; set; }
        public int SurveysCompleted { get; set; }
        public decimal AccuracyRate { get; set; }
    }

    public class ComplianceOfficer : User
    {
        public string CertificationLevel { get; set; }
        public int AuditsCompleted { get; set; }
        public string ComplianceStatus { get; set; }
    }

    public class Policy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public string Description { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public string PolicyType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ClaimsOfficersCount { get; set; }
        public int SurveyOfficersCount { get; set; }
        public int ComplianceOfficersCount { get; set; }
        public int CustomersCount { get; set; }
        public List<ClaimsOfficer> ClaimsOfficers { get; set; } = new();
        public List<SurveyOfficer> SurveyOfficers { get; set; } = new();
        public List<ComplianceOfficer> ComplianceOfficers { get; set; } = new();
        public List<User> AllUsers { get; set; } = new();
        public List<Policy> Policies { get; set; } = new();
    }

    // =============== AUTHENTICATION MODELS ===============

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCredential
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
    }

    public class AuthenticatedUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public DateTime LoginTime { get; set; }
    }
}

