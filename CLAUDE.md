# CLAUDE.md — Total Operations Database (TOD/AOM)

## Project Overview

**Total Operations Database (TOD)** is an enterprise ASP.NET Blazor Server application for operations management. It handles employee records, workforce management, ACR (Allowance Change Requests), Opera (Operational Requests), training/certifications, and various operational tools.

**Org:** Leidos NMCI
**Deployment:** Azure App Service (GCC High environment)
**Auth:** Microsoft Entra ID (Azure AD) via OpenID Connect

---

## Tech Stack

- **.NET 10.0** — Blazor Server (`net10.0`, `EnableWindowsTargeting`)
- **MudBlazor 9.x** — Material Design UI components (dark/light theme toggle)
- **Entity Framework Core 10.0** — ORM with two separate DbContexts
- **Dapper 2.1** — Lightweight SQL for complex queries
- **Microsoft.Identity.Web 4.x** — Entra ID / OIDC authentication
- **Microsoft Graph 5.x** — GCC High endpoint (`graph.microsoft.us`) for group membership and mail
- **TimeZoneConverter 7.x** — Cross-platform timezone handling
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

# Publish
dotnet publish MyApplication -c Release -o ./publish
```

---

## Project Structure

```
MudBlazor/
├── AOM/                        # Documentation (Obsidian vault)
├── AddHomeEvent.sql            # Migration SQL script
├── CLAUDE.md
├── MudBlazor.sln
└── MyApplication/
    ├── Areas/
    │   └── MicrosoftIdentity/Pages/Account/  # SignedOut.cshtml.cs
    ├── Components/
    │   ├── Common/
    │   │   ├── Auth/           # RoleConstants.cs, AuthorizationHelpers.cs
    │   │   ├── IdentityHelper.cs
    │   │   ├── KvSearchHelper.cs
    │   │   └── Time/           # Et, Mt, SiteTime, Tz, TimeDisplayService, TimeInputHelpers
    │   ├── Data/
    │   │   ├── AomDbContext.cs          # Main DB context (factory pattern, split into partials)
    │   │   ├── AomDbContext.Aws.cs      # Aws entity mappings
    │   │   ├── AomDbContext.Employee.cs # Employee entity mappings
    │   │   ├── AomDbContext.Security.cs # Security entity mappings
    │   │   ├── AomDbContext.Tools.cs    # Tools entity mappings (incl. HomeEvent)
    │   │   ├── AwsDbContext.cs          # AWS contact center DB (scoped, auto-generated)
    │   │   └── Configurations/          # EF entity type configurations
    │   ├── Layout/             # MainLayout.razor, NavMenu.razor
    │   ├── Model/
    │   │   ├── AOM/
    │   │   │   ├── Aws/        # CallQueue, RoutingProfile, Status, etc.
    │   │   │   ├── Employee/   # Employees, Manager, Supervisor, Acr, Opera, Skills, etc.
    │   │   │   │   └── Views/  # EmployeeCurrentDetails (DB view model)
    │   │   │   └── Tools/      # EmailTemplates, HomeEvent, IntervalSummary, OI, OstPassdown, Proactive, Security
    │   │   └── AWS/            # Contact center models (54+ auto-generated)
    │   ├── Pages/
    │   │   ├── Account/        # AccessDenied, Logout, SignedOut
    │   │   ├── Acr/            # Index, New, Edit, Details + Forms/ + Shared/
    │   │   ├── Admin/          # DebugClaims, Roles/ (Index, RoleAssignmentDialog)
    │   │   ├── Employees/      # Index, Details, Schedules, User, EmployeeRoutingAssignment, Dialogs/
    │   │   ├── Opera/          # Index, Submit, Details
    │   │   ├── Tools/          # EmailTemplatesPage/, Interval/, OperationalImpact/, OstPassdown/, Proactive/
    │   │   ├── Training/       # Certifications/ (Index, Details, Upload), Skills/ (SkillsLookup, AddSkill)
    │   │   ├── Wfm/            # BreakTemplatesPage, StaticBreaksPage, Aws/ (BulkAssignment, Profiles/, Queues/)
    │   │   ├── Home.razor      # Dashboard with Home.razor.cs code-behind
    │   │   └── HomeEventDialog.razor  # Home event CRUD dialog
    │   ├── Service/
    │   │   ├── Acr/            # AcrService, AcrQueryService (split: Details, Lookups, Search), Dtos/
    │   │   ├── Aws/            # AwsRoutingService
    │   │   ├── Email/          # EmailComposer, builders (Interval, OI, Proactive), TemplateRenderer
    │   │   ├── Employee/       # EmployeeDetailsService, EmployeeListService, EmployeeScheduleService, ViewModels/
    │   │   ├── Home/           # HomeEventService, HomeDashboardCache
    │   │   ├── Opera/          # OperaRepository (split: Commands, Queries)
    │   │   ├── Security/       # SecurityService
    │   │   ├── Tools/
    │   │   │   ├── Interval/   # IntervalSummaryRepository, IntervalEmailService, IntervalSummaryState
    │   │   │   ├── Oi/         # OiEventRepository, OiLookupRepository
    │   │   │   ├── OstPassdown/# OstPassdownService
    │   │   │   └── Proactive/  # ProactiveRepository
    │   │   ├── Training/       # Certifications/ (CertificationsRepository), Skills/ (SkillsService)
    │   │   ├── WFM/            # WfmService
    │   │   ├── ClaimsEnrichmentMiddleware.cs
    │   │   ├── EntraIdClaimsEnricher.cs
    │   │   ├── UserProfileService.cs
    │   │   └── WindowsClaimsEnricher.cs
    │   └── Shared/
    │       ├── Base/           # AppComponentBase.cs
    │       ├── DataGrid/       # AppDataGrid.razor
    │       ├── Filters/        # MultiSelectFilter.razor, SearchBox.razor
    │       └── LoadingSpinner.razor
    ├── Controllers/            # CertificationsController (file uploads)
    ├── Infrastructure/
    │   └── IDbConnectionFactory.cs  # Multi-database connection factory
    ├── Migrations/             # AomDbContext EF migrations
    ├── Program.cs
    ├── appsettings.json
    └── MyApplication.csproj
```

---

## Database Architecture

Two separate SQL Server databases:

| Context | Database | Usage | DI Pattern |
|---------|----------|-------|-----------|
| `AomDbContext` | `Aom` | Main app data | **Factory** (`IDbContextFactory`) — required for Blazor Server |
| `AwsDbContext` | `TOdNMCIAWS` | AWS Connect analytics (read-mostly) | Scoped `DbContext` |

### AomDbContext Schemas
- `Employee` — Employees, managers, supervisors, sites, orgs, schedules, skills, break templates, certifications, overtime
- `Tools` — Email templates, interval reports, operational impact, OST passdowns, proactive, **HomeEvent**
- `Security` — `AppRole` (6 roles), `AppRoleAssignment` (user or group assignments)
- `Aws` — CallQueues, RoutingProfiles, RoutingProfileQueues, EmployeeRoutingProfiles, Statuses

### AomDbContext is split into partial classes:
- `AomDbContext.cs` — base context
- `AomDbContext.Employee.cs` — Employee schema entities
- `AomDbContext.Tools.cs` — Tools schema entities (incl. HomeEvent)
- `AomDbContext.Security.cs` — Security schema entities
- `AomDbContext.Aws.cs` — Aws schema entities

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

All services registered in `Program.cs`:

| Service | Interface | Lifetime | Module |
|---------|-----------|----------|--------|
| TimeDisplayService | ITimeDisplayService | Scoped | Common/Time |
| EmailComposer | IEmailComposer | Scoped | Email |
| UserProfileService | — | Scoped | Core |
| EmployeeDetailsService | — | Scoped | Employee |
| EmployeeListService | — | Scoped | Employee |
| EmployeeScheduleService | — | Scoped | Employee |
| SkillsService | ISkillsService | Scoped | Training |
| AcrService | IAcrService | Scoped | ACR |
| AcrQueryService | IAcrQueryService | Scoped | ACR |
| OperaRepository | IOperaRepository | Scoped | Opera |
| WfmService | IWfmService | Scoped | WFM |
| IntervalSummaryRepository | IIntervalSummaryRepository | Scoped | Tools/Interval |
| IntervalEmailService | — | Scoped | Tools/Interval |
| OiLookupRepository | IOiLookupRepository | Scoped | Tools/OI |
| OiEventRepository | IOiEventRepository | Scoped | Tools/OI |
| OperationalImpactEmailService | — | Scoped | Tools/OI |
| ProactiveRepository | IProactiveRepository | Scoped | Tools/Proactive |
| CertificationsRepository | ICertificationsRepository | Scoped | Training |
| HomeEventService | IHomeEventService | Scoped | Home |
| HomeDashboardCache | — | **Singleton** | Home |
| SecurityService | — | Scoped | Security |
| AwsRoutingService | — | Scoped | AWS |
| OstPassdownService | IOstPassdownService | Transient | Tools/OstPassdown |

- Email namespace: `MyApplication.Components.Service.Email` (singular — no 's')
- Do NOT call `ProactiveEmailBuilder.Build` directly from pages — use `IEmailComposer.ComposeAsync` only
- `AcrQueryService` is split into partials: `.Details.cs`, `.Lookups.cs`, `.Search.cs`
- `OperaRepository` is split into partials: `.Commands.cs`, `.Queries.cs`

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
| Home | Dashboard, HomeEventDialog | FullAccess |
| Employees | Index, Details, Schedules, User, EmployeeRoutingAssignment | FullAccess |
| ACR | Index, New, Edit, Details + 7 form types | FullAccess |
| Opera | Index, Submit, Details | FullAccess |
| WFM | BreakTemplates, StaticBreaks, Aws/ (BulkAssignment, Profiles, Queues) | Admin/WFM/OST |
| Training | Skills (Lookup, Add), Certifications (Index, Details, Upload) | Training/FullAccess |
| Tools | Intervals, OI, Passdown, Proactive, EmailTemplates | Varies |
| Admin | Role Assignment, Debug Claims | Admin |
| Account | AccessDenied, Logout, SignedOut | — |

---

## Deployment Notes

- **Azure App Service** — primary target (HTTPS, no local cert)
- **Windows Service** — supported via `UseWindowsService()`
- No `--no-verify` or force-push to `master`
- Current active branch: `AzureNew`; main branch: `master`
- Health endpoint available at `/healthz`
- Distributed SQL Server token cache for MSAL token persistence
