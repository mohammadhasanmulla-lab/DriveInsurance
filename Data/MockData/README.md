# Admin Panel - DriveSure Insurance

## Overview
Complete admin management system for DriveSure Insurance application with mock data.

## Features

### 1. **Admin Dashboard** (`/Home/AdminDashboard`)
- **Overview Statistics**
  - Total Users Count
  - Claims Officers Count
  - Survey Officers Count
  - Compliance Officers Count
  - Customers Count

- **Quick Access Tables**
  - Claims Officers Summary
  - Survey Officers Summary
  - Compliance Officers Summary
  - Active Policies List

- **Quick Actions**
  - Manage Users
  - Manage Policies
  - Add New Policy

### 2. **User Management**
- **View All Users** (`/Home/ManageUsers`)
  - Display all system users
  - View user details (ID, Username, Email, Role, Status)
  - Filter by role and status
  - **Delete Users** - Remove any non-admin user from the system

### 3. **Claims Officers Management** (`/Home/ClaimsOfficers`)
- View all claims officers with details
- See department assignments
- Track claims handled
- View approval rates (%)
- Performance metrics

### 4. **Survey Officers Management** (`/Home/SurveyOfficers`)
- View all survey officers
- See specialization areas
- Track surveys completed
- View accuracy rates (%)
- Performance tracking

### 5. **Compliance Officers Management** (`/Home/ComplianceOfficers`)
- View all compliance officers
- See certification levels
- Track audits completed
- Monitor compliance status
- Compliance reporting

### 6. **Policy Management**
- **View All Policies** (`/Home/ManagePolicies`)
  - Display all insurance policies
  - See policy details (name, type, coverage, premium)
  - Policy status (Active/Inactive)
  - Edit and delete policies

- **Add New Policy** (`/Home/AddPolicy`)
  - Create new insurance policies
  - Set policy type (Auto, Health, Home, Life, Travel, Business)
  - Configure coverage amount and premium
  - Activate/Deactivate policies
  - Form validation

## Mock Data Structure

### Data Files Location: `Data/MockData/`

#### Users
- **Count**: 8 users
- **Roles**: Customer, ClaimsOfficer, Surveyor, ComplianceOfficer, Admin

#### Claims Officers
- **Count**: 2 officers
- **Tracked Metrics**: Claims Handled, Approval Rate

#### Survey Officers
- **Count**: 2 officers
- **Tracked Metrics**: Surveys Completed, Accuracy Rate

#### Compliance Officers
- **Count**: 1 officer
- **Tracked Metrics**: Audits Completed, Compliance Status

#### Policies
- **Count**: 4 policies
- **Types**: Auto, Health, Home, Travel
- **Info**: Coverage Amount, Premium, Description

## File Structure

```
/Controllers
  └── HomeController.cs (Contains all admin actions and mock data)

/Views/Home
  ├── AdminDashboard.cshtml (Main dashboard)
  ├── ClaimsOfficers.cshtml (Claims officers list)
  ├── SurveyOfficers.cshtml (Survey officers list)
  ├── ComplianceOfficers.cshtml (Compliance officers list)
  ├── ManageUsers.cshtml (User management)
  ├── ManagePolicies.cshtml (Policy list)
  └── AddPolicy.cshtml (Policy creation form)

/Models
  └── ErrorViewModel.cs (Contains all admin models and ViewModels)

/Data/MockData
  ├── Users.json (User mock data reference)
  ├── ClaimsOfficers.json (Claims officers reference)
  ├── SurveyOfficers.json (Survey officers reference)
  ├── ComplianceOfficers.json (Compliance officers reference)
  └── Policies.json (Policies reference)
```

## Admin Models

### User
- Id, Username, Email, Role, CreatedDate, IsActive

### ClaimsOfficer : User
- Department, ClaimsHandled, ApprovalRate

### SurveyOfficer : User
- Specialization, SurveysCompleted, AccuracyRate

### ComplianceOfficer : User
- CertificationLevel, AuditsCompleted, ComplianceStatus

### Policy
- Id, PolicyName, Description, CoverageAmount, PremiumAmount, PolicyType, CreatedDate, IsActive

### AdminDashboardViewModel
- Aggregate statistics and collections of all entity types

## Current Implementation

### Mock Data Features
- All data is currently stored in memory within controller methods
- Each method generates fresh mock data on request
- Mock data includes realistic values for all metrics

### Available Actions

| Action | URL | Method | Description |
|--------|-----|--------|-------------|
| Dashboard | `/Home/AdminDashboard` | GET | Main admin dashboard |
| View Claims Officers | `/Home/ClaimsOfficers` | GET | Claims officers list |
| View Survey Officers | `/Home/SurveyOfficers` | GET | Survey officers list |
| View Compliance Officers | `/Home/ComplianceOfficers` | GET | Compliance officers list |
| Manage Users | `/Home/ManageUsers` | GET | User management interface |
| Delete User | `/Home/DeleteUser/{id}` | GET | Delete a specific user |
| View Policies | `/Home/ManagePolicies` | GET | Policy management interface |
| Add Policy | `/Home/AddPolicy` | GET/POST | Add new policy form and submission |

## Future Enhancements

1. **Database Integration**
   - Replace mock data with EF Core database
   - Implement persistent data storage

2. **Advanced Features**
   - User authentication and authorization
   - Role-based access control (RBAC)
   - Policy filtering and search
   - User activity logs
   - Advanced reporting and analytics
   - Export to CSV/PDF

3. **User Management**
   - Edit user profiles
   - Reset user passwords
   - Bulk user operations
   - User activity history

4. **Policy Management**
   - Policy version control
   - Policy templates
   - Discount management
   - Policy renewal tracking

## Testing the Admin Panel

1. Navigate to `/Home/AdminDashboard` to access the main dashboard
2. Use the dashboard to navigate to specific sections
3. Try the delete user functionality (non-admin users only)
4. Try adding a new policy through the form
5. All operations currently show success messages

## Notes
- Mock data is regenerated on each page load
- Delete operations show confirmation but don't persist
- Policy additions show success message but don't persist
- All data will reset if the application restarts
- For production, replace with actual database implementation
