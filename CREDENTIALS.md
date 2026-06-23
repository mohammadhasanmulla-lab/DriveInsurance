# DriveSure Insurance - Login Credentials

## Test Credentials for All User Types

> ⚠️ **Important**: These are mock credentials for testing only. Never use in production!

---

## 1. 🔐 Admin Account

| Field | Value |
|-------|-------|
| **Email** | `admin@company.com` |
| **Username** | `admin_user` |
| **Password** | `Admin@123` |
| **Role** | Admin |
| **Full Name** | Admin User |
| **Department** | Administration |

**Access Level**: Full admin panel, user management, policy management

---

## 2. 📋 Claims Officers

### Claims Officer 1
| Field | Value |
|-------|-------|
| **Email** | `jane@company.com` |
| **Username** | `jane_smith` |
| **Password** | `Jane@123` |
| **Role** | ClaimsOfficer |
| **Full Name** | Jane Smith |
| **Department** | Auto Claims |

### Claims Officer 2
| Field | Value |
|-------|-------|
| **Email** | `emily@company.com` |
| **Username** | `emily_davis` |
| **Password** | `Emily@123` |
| **Role** | ClaimsOfficer |
| **Full Name** | Emily Davis |
| **Department** | Health Claims |

**Access Level**: View claims, process claims, manage claim documents

---

## 3. 🔍 Survey Officers

### Survey Officer 1
| Field | Value |
|-------|-------|
| **Email** | `mike@company.com` |
| **Username** | `mike_wilson` |
| **Password** | `Mike@123` |
| **Role** | Surveyor |
| **Full Name** | Mike Wilson |
| **Department** | Survey Department |

### Survey Officer 2
| Field | Value |
|-------|-------|
| **Email** | `david@company.com` |
| **Username** | `david_miller` |
| **Password** | `David@123` |
| **Role** | Surveyor |
| **Full Name** | David Miller |
| **Department** | Survey Department |

**Access Level**: View survey requests, submit survey reports, view assignments

---

## 4. ✅ Compliance Officer

| Field | Value |
|-------|-------|
| **Email** | `sara@company.com` |
| **Username** | `sara_johnson` |
| **Password** | `Sara@123` |
| **Role** | ComplianceOfficer |
| **Full Name** | Sara Johnson |
| **Department** | Compliance |

**Access Level**: View compliance reports, audit logs, policy compliance

---

## 5. 👤 Customer

| Field | Value |
|-------|-------|
| **Email** | `john@company.com` |
| **Username** | `john_doe` |
| **Password** | `John@123` |
| **Role** | Customer |
| **Full Name** | John Doe |
| **Department** | N/A |

**Access Level**: View policies, submit claims, view claim status

---

## Quick Login Guide

### To Login:
1. Go to `/Home/Login`
2. Enter **Email** and **Password** from the table above
3. Click **Sign In**
4. You'll be redirected to your dashboard

### Quick Test Logins

**For Admin Testing:**
```
Email: admin@company.com
Password: Admin@123
```

**For Claims Officer Testing:**
```
Email: jane@company.com
Password: Jane@123
```

**For Survey Officer Testing:**
```
Email: mike@company.com
Password: Mike@123
```

**For Compliance Officer Testing:**
```
Email: sara@company.com
Password: Sara@123
```

**For Customer Testing:**
```
Email: john@company.com
Password: John@123
```

---

## Password Policy

- All passwords follow format: `Name@123`
- Minimum requirements (for reference):
  - Length: 8 characters
  - Contains: Uppercase letter, special character, numbers

---

## File Location

- **Credentials Storage**: `Data/MockData/Credentials.json`
- **Login Page**: `/Home/Login`

---

## Notes

- Credentials are stored in memory and mock JSON (not a real database)
- Each user role has specific access permissions
- Login system uses session-based authentication
- Passwords are case-sensitive
- All credentials are for testing/demo purposes only

---

## Testing Scenarios

### Scenario 1: Admin Access
- Login with admin credentials
- Access admin dashboard
- Manage users and policies

### Scenario 2: Claims Officer Workflow
- Login as jane_smith
- View assigned claims
- Process and update claims

### Scenario 3: Survey Officer Workflow
- Login as mike_wilson
- View survey assignments
- Submit survey reports

### Scenario 4: Customer Experience
- Login as john_doe
- View purchased policies
- Submit insurance claim

---

**Last Updated**: 2024
**Version**: 1.0
