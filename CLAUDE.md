# CLAUDE.md — Total Operations Database (TOD/AOM)

## Project Overview

**Total Operations Database (TOD)** is an enterprise ASP.NET Blazor Server application for operations management. It handles employee records, workforce management, ACR (Allowance Change Requests), Opera (Operational Requests), training/certifications, and various operational tools.

**Org:** Leidos NMCI
**Deployment:** Azure App Service (GCC High environment)
**Auth:** Microsoft Entra ID (Azure AD) via OpenID Connect

---

## Tech Stack

- **.NET 9.0** — Blazor Server (`net9.0`, `EnableWindowsTargeting`)
- **MudBlazor 8.x** — Material Design UI components (dark/light theme toggle)
- **Entity Framework Core 9.0** — ORM with two separate DbContexts
- **Dapper** — Lightweight SQL for complex queries
- **Microsoft.Identity.Web** — Entra ID / OIDC authentication
- **Microsoft Graph API** — GCC High endpoint (`graph.microsoft.us`) for group membership and mail
- **SQL Server** — Two databases on `NmciSql.corp.leidos.com`

---

## Key Commands

```bash
# Run locally
dotnet run --project MyApplication

# Build
dotnet build

# EF migrations (run from MyApplication/)
dotnet ef migrations add <MigrationName> --context AomDbContext
dotnet ef database update --context AomDbContext
```

---

## Project Structure

```
MudBlazor/
├── MyApplication/
│   ├── Components/
│   │   ├── Common/
│   │   │   ├── Auth/           # RoleConstants.cs, AuthorizationHelpers.cs
│   │   │   └── Time/           # Multi-timezone utilities (ET, MT, SiteTime)
│   │   ├── Data/
│   │   │   ├── AomDbContext.cs     # Main app DB context (factory pattern)
│   │   │   └── AwsDbContext.cs     # AWS contact center DB (scoped, auto-generated)
│   │   ├── Layout/             # MainLayout.razor, NavMenu.razor
│   │   ├── Model/
│   │   │   ├── AOM/            # App domain models (Employee, Aws, Tools)
│   │   │   └── AWS/            # Contact center models (auto-generated)
│   │   ├── Pages/
│   │   │   ├── Acr/            # Allowance Change Requests
│   │   │   ├── Admin/          # Role management, DebugClaims
│   │   │   ├── Employees/      # Employee directory, schedules, user profile
│   │   │   ├── Opera/          # Operational Requests
│   │   │   ├── Tools/          # Interval, OstPassdown, Proactive, OI, EmailTemplates
│   │   │   ├── Training/       # Skills & Certifications
│   │   │   └── Wfm/Aws/        # Workforce mgmt, queues, routing profiles
│   │   ├── Service/            # Business logic repositories and services
│   │   │   ├── Employee/       # EmployeeDetailsService, EmployeeListService, EmployeeScheduleService
│   │   │   │   └── ViewModels/ # EmployeeFullDetailsVm, SchedulesVm
│   │   │   ├── Email/          # EmailComposer, builders, contexts (namespace: Service.Email)
│   │   │   ├── ClaimsEnrichmentMiddleware.cs
│   │   │   ├── EntraIdClaimsEnricher.cs
│   │   │   └── UserProfileService.cs
│   │   └── Shared/             # Reusable Razor components (dialogs, grids)
│   ├── Infrastructure/
│   │   └── IDbConnectionFactory.cs  # Multi-database connection factory
│   ├── Controllers/            # API endpoints (e.g., file uploads)
│   ├── Migrations/             # AomDbContext EF migrations
│   ├── Program.cs
│   ├── appsettings.json
│   └── MyApplication.csproj
└── MudBlazor.sln
```

---

## Database Architecture

Two separate SQL Server databases:

| Context | Database | Usage | DI Pattern |
|---------|----------|-------|-----------|
| `AomDbContext` | `Aom` | Main app data | **Factory** (`IDbContextFactory`) — required for Blazor Server |
| `AwsDbContext` | `TOdNMCIAWS` | AWS Connect analytics (read-mostly) | Scoped `DbContext` |

### AomDbContext Schemas
- `Employee` — Employees, managers, supervisors, sites, orgs, schedules, skills, break templates
- `Tools` — Email templates, interval reports, operational impact, OST passdowns, proactive
- `Security` — `AppRole` (6 roles), `AppRoleAssignment` (user or group assignments)

### Important: Always use `IDbContextFactory<AomDbContext>` in components and services, not direct `AomDbContext` injection. This is required for Blazor Server's concurrent request model.

---

## Authentication & Authorization

### Flow
1. Entra ID OIDC sign-in
2. `ClaimsEnrichmentMiddleware` runs post-auth:
   - Calls Graph API (`graph.microsoft.us`) for group memberships
   - Queries `Security.AppRoleAssignment` table
   - Adds role claims to user principal

### Roles (defined in `RoleConstants.cs`)
Individual: `Admin`, `OST`, `WFM`, `Manager`, `Supervisor`, `TechLead`, `Training`
Combinations: `FullAccess`, `Management`, `AdminOst`, etc.

### Usage patterns
```csharp
// In components — use AuthorizationHelpers
var roles = await authProvider.GetUserRolesAsync();
if (roles.IsAdmin) { ... }

// Razor — use policy or role attribute
[Authorize(Policy = "Admin")]
// or
<AuthorizeView Roles="@RoleConstants.Admin">
```

### Authorization Policies (Program.cs)
- `"Admin"` → requires Admin role
- `"OST"` → requires Admin OR OST role

---

## Key Patterns & Conventions

### Components
- Blazor Server with `@rendermode InteractiveServer`
- All pages inherit `AppComponentBase` (`Components/Shared/Base/AppComponentBase.cs`); `AppDataGrid` for data grids
- `AppComponentBase` provides: `ShowSuccess/Error/Warning/Info`, `NavigateTo`, `ConfirmAsync`, `GetUserRolesAsync()`, `RunAsync(Func<Task>)` + `IsLoading`
- Loading pattern: `await RunAsync(async () => { ... })` + `@if (IsLoading)` in markup — no manual `_loading` bool
- Role pattern: `var roles = await GetUserRolesAsync();` or `private UserRoles _roles` for per-row checks
- MudBlazor snackbar for notifications — use base class helper methods, not direct calls
- `KvSearchHelper.Search(source, value)` in `Components/Common/` for MudAutocomplete SearchFunc over `KeyValuePair<int,string>`

### Services / Repositories
- Employee services: `EmployeeDetailsService`, `EmployeeListService`, `EmployeeScheduleService` (all in `Service/Employee/`)
- Other repositories: `AcrService` + `AcrQueryService`, `OperaRepository`, `SkillsService`, etc.
- Email services: `IEmailComposer` / `EmailComposer` (template rendering), `IntervalEmailService`, `OiEmailService`
- Email namespace: `MyApplication.Components.Service.Email` (singular — no 's')
- Do NOT call `ProactiveEmailBuilder.Build` directly from pages — use `IEmailComposer.ComposeAsync` only

### Nullable Reference Types
- **Enabled** (`<Nullable>enable</Nullable>`) — respect nullability annotations

### Time Zones
- App is multi-timezone aware — use utilities in `Components/Common/Time/`
- Do not use raw `DateTime.Now`; use site-aware time helpers

### EF Core Best Practices in This Codebase
- Prefer `await using var db = await _dbFactory.CreateDbContextAsync()` in services
- Use `AsNoTracking()` for read-only queries
- Database views available: `EmployeeCurrentDetails`, `vw_DailyScheduleDetails`

---

## Modules Summary

| Module | Pages | Min Role |
|--------|-------|----------|
| Employees | Index, Details, Schedules, User | FullAccess |
| ACR | Index, New, Edit, Details | FullAccess |
| Opera | Index, Submit, Details | FullAccess |
| WFM/AWS | Queues, Profiles, Bulk Assignment | Admin/WFM/OST |
| Training | Skills, Certifications | Training/FullAccess |
| Tools | Intervals, OI, Passdown, Proactive, EmailTemplates | Varies |
| Admin | Role Assignment, Debug Claims | Admin |

---

## Deployment Notes

- **Azure App Service** — primary target (HTTPS, no local cert)
- **Windows Service** — supported via `UseWindowsService()`
- No `--no-verify` or force-push to `master`
- Current active branch: `AzureNew`; main branch: `master`
- Health endpoint available at `/health`
