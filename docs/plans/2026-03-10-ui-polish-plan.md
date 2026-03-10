# UI Polish v1.2 Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Transform the TOD app from stock MudBlazor to a polished modern SaaS-style internal ops tool using the Inter font, shared layout components, KPI stat cards on the dashboard, and consistent card/header patterns across all pages.

**Architecture:** Three new shared Razor components (`AppCard`, `StatCard`, `SectionHeader`) are created in `Components/Shared/` and auto-imported via the existing `@using MyApplication.Components.Shared` in `_Imports.razor`. Global CSS changes in `app.css` apply immediately to every page. All data/logic remains unchanged — this is purely UI.

**Tech Stack:** Blazor Server, MudBlazor 9.x, CSS custom properties, Inter font (Google Fonts CDN)

---

## Task 1: Font + CSS Foundation

**Files:**
- Modify: `MyApplication/Components/App.razor:10`
- Modify: `MyApplication/wwwroot/css/app.css`

**Step 1: Replace the Roboto font link with Inter in `App.razor`**

Replace line 10:
```html
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
```
With:
```html
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
```

**Step 2: Add global CSS rules to the bottom of `app.css`**

Append the following block (add after the existing `#blazor-error-ui .dismiss:hover` rule):

```css

/* ── 10. Typography — Inter font ──────────────────────────── */

body {
    font-family: 'Inter', sans-serif;
}


/* ── 11. Border Radius ────────────────────────────────────── */

.mud-paper {
    border-radius: 8px !important;
}


/* ── 12. Card / Section Header ───────────────────────────── */

/* Header row inside AppCard */
.app-card-header {
    display: flex;
    align-items: center;
    padding-bottom: 10px;
    margin-bottom: 12px;
    border-bottom: 1px solid var(--mud-palette-divider);
}

/* Page-level section title divider */
.section-header {
    padding-bottom: 12px;
    border-bottom: 1px solid var(--mud-palette-divider);
    margin-bottom: 16px;
}


/* ── 13. Stat Cards ──────────────────────────────────────── */

.stat-card {
    border-radius: 8px;
    transition: box-shadow 0.15s ease;
}

.stat-card:hover {
    box-shadow: var(--mud-elevation-6) !important;
}


/* ── 14. Modern Table Styles ─────────────────────────────── */

/* Applied via Class="table-modern" on MudSimpleTable */
.table-modern thead th {
    font-weight: 600;
    font-size: 0.72rem;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    color: var(--mud-palette-text-secondary);
    padding: 8px 12px;
}

.table-modern tbody td {
    padding: 9px 12px;
    border-bottom: 1px solid var(--mud-palette-divider);
    font-size: 0.875rem;
}

.table-modern tbody tr:last-child td {
    border-bottom: none;
}

.table-modern tbody tr:hover td {
    background-color: var(--mud-palette-background-gray);
}


/* ── 15. Main content breathing room ────────────────────── */

.mud-main-content {
    padding: 72px 24px 24px !important;
}
```

**Step 3: Build to verify no CSS syntax issues**

Run: `dotnet build MyApplication`
Expected: "Build succeeded" (ignore MSB3026 file-lock warnings if app is running)

**Step 4: Commit**

```bash
git add MyApplication/Components/App.razor MyApplication/wwwroot/css/app.css
git commit -m "feat(ui): switch to Inter font, add CSS foundation for v1.2 polish"
```

---

## Task 2: AppCard Shared Component

**Files:**
- Create: `MyApplication/Components/Shared/AppCard.razor`

**Step 1: Create the file**

```razor
@* AppCard — standard elevated card with optional titled header and actions slot.
   Auto-imported via @using MyApplication.Components.Shared in _Imports.razor.

   Usage (title only):
       <AppCard Title="My Section">content</AppCard>

   Usage (title + actions):
       <AppCard Title="My Section">
           <Actions><MudIconButton .../></Actions>
           <ChildContent>content</ChildContent>
       </AppCard>

   Usage (no title, just elevated paper):
       <AppCard>content</AppCard>
*@

<MudPaper Elevation="3" Class="@Class">
    @if (!string.IsNullOrEmpty(Title))
    {
        <div class="app-card-header">
            @if (TitleIcon is not null)
            {
                <MudIcon Icon="@TitleIcon" Color="@TitleColor" Size="Size.Small" Class="mr-2" />
            }
            <MudText Typo="Typo.subtitle1" Style="font-weight:600;flex:1">@Title</MudText>
            @Actions
        </div>
    }
    @ChildContent
</MudPaper>

@code {
    /// <summary>Card title displayed in the header row. Omit for a bare elevated card.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Optional MudBlazor icon string shown left of the title.</summary>
    [Parameter] public string? TitleIcon { get; set; }

    /// <summary>Icon color. Default = Color.Default.</summary>
    [Parameter] public Color TitleColor { get; set; } = Color.Default;

    /// <summary>Content rendered on the right side of the header row (buttons, chips, etc.).</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    /// <summary>Card body content.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>CSS class applied to the root MudPaper. Default = "pa-4".</summary>
    [Parameter] public string Class { get; set; } = "pa-4";
}
```

**Step 2: Build to verify it compiles**

Run: `dotnet build MyApplication`
Expected: Build succeeded, 0 errors

**Step 3: Commit**

```bash
git add MyApplication/Components/Shared/AppCard.razor
git commit -m "feat(ui): add AppCard shared component"
```

---

## Task 3: StatCard Shared Component

**Files:**
- Create: `MyApplication/Components/Shared/StatCard.razor`

**Step 1: Create the file**

```razor
@* StatCard — KPI summary card for dashboard stat rows.
   Auto-imported via @using MyApplication.Components.Shared in _Imports.razor.

   Usage:
       <StatCard Value="1,842" Label="Calls Today"
                 Icon="@Icons.Material.Filled.Phone" Color="Color.Primary" />
*@

<MudPaper Elevation="3" Class="stat-card pa-4">
    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="3">
        <MudAvatar Color="@Color" Variant="Variant.Rounded" Size="Size.Large">
            <MudIcon Icon="@Icon" />
        </MudAvatar>
        <MudStack Spacing="0">
            <MudText Typo="Typo.h5" Style="font-weight:700;line-height:1.1">@Value</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">@Label</MudText>
            @if (Subtitle is not null)
            {
                <MudText Typo="Typo.caption" Color="Color.Secondary">@Subtitle</MudText>
            }
        </MudStack>
    </MudStack>
</MudPaper>

@code {
    [Parameter, EditorRequired] public string Value { get; set; } = "";
    [Parameter, EditorRequired] public string Label { get; set; } = "";
    [Parameter, EditorRequired] public string Icon { get; set; } = "";
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public string? Subtitle { get; set; }
}
```

**Step 2: Build to verify**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add MyApplication/Components/Shared/StatCard.razor
git commit -m "feat(ui): add StatCard shared component"
```

---

## Task 4: SectionHeader Shared Component

**Files:**
- Create: `MyApplication/Components/Shared/SectionHeader.razor`

**Step 1: Create the file**

```razor
@* SectionHeader — page-level title area with optional subtitle and actions.
   Replaces the ad-hoc MudStack Row + MudText h5 + MudSpacer + buttons pattern.
   Auto-imported via @using MyApplication.Components.Shared in _Imports.razor.

   Usage:
       <SectionHeader Title="Employees" />

   Usage with subtitle + button:
       <SectionHeader Title="ACRs" Subtitle="Allowance Change Requests">
           <Actions>
               <MudButton ...>New Request</MudButton>
           </Actions>
       </SectionHeader>
*@

<div class="section-header mb-4">
    <MudStack Row="true" AlignItems="AlignItems.Center">
        <MudStack Spacing="0" Style="flex:1">
            <MudText Typo="Typo.h5">@Title</MudText>
            @if (!string.IsNullOrEmpty(Subtitle))
            {
                <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mt-1">@Subtitle</MudText>
            }
        </MudStack>
        @Actions
    </MudStack>
</div>

@code {
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter] public string? Subtitle { get; set; }
    [Parameter] public RenderFragment? Actions { get; set; }
}
```

**Step 2: Build to verify**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add MyApplication/Components/Shared/SectionHeader.razor
git commit -m "feat(ui): add SectionHeader shared component"
```

---

## Task 5: NavMenu — Branding Header + Version Badge

**Files:**
- Modify: `MyApplication/Components/Layout/NavMenu.razor`

**Step 1: Add nav branding CSS to `app.css`**

Append to `app.css`:
```css

/* ── 16. Navigation Branding ─────────────────────────────── */

.nav-brand {
    display: flex;
    flex-direction: column;
    padding: 16px 16px 12px;
    border-bottom: 1px solid var(--mud-palette-divider);
    margin-bottom: 8px;
}

.nav-brand-logo {
    font-size: 1.25rem;
    font-weight: 700;
    color: var(--mud-palette-primary);
    letter-spacing: 0.05em;
}

.nav-brand-sub {
    font-size: 0.65rem;
    color: var(--mud-palette-text-secondary);
    text-transform: uppercase;
    letter-spacing: 0.06em;
    margin-top: 2px;
}

.nav-version {
    padding: 6px 16px 10px;
    font-size: 0.7rem;
    color: var(--mud-palette-text-disabled);
}
```

**Step 2: Add the branding block at the top of `NavMenu.razor`**

Replace the existing `<MudNavMenu>` opening line with:
```razor
<div class="nav-brand">
    <span class="nav-brand-logo">TOD</span>
    <span class="nav-brand-sub">Total Operations Database</span>
</div>

<MudNavMenu>
```

**Step 3: Add the version badge at the very bottom of `NavMenu.razor`**

After the closing `</MudNavMenu>` tag, add:
```razor
<div class="nav-version">v1.2</div>
```

**Step 4: Build**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add MyApplication/Components/Layout/NavMenu.razor MyApplication/wwwroot/css/app.css
git commit -m "feat(ui): add TOD branding header and version badge to nav sidebar"
```

---

## Task 6: Home Dashboard — KPI Stat Row

**Files:**
- Modify: `MyApplication/Components/Pages/Home.razor`
- Modify: `MyApplication/Components/Pages/Home.razor.cs`

**Step 1: Add computed KPI properties to `Home.razor.cs`**

Add the following computed properties after the field declarations (after line 37, before `OnInitializedAsync`):

```csharp
// ── KPI stat row computed properties ─────────────────────
private int TotalCallsToday => _dailyStats.Sum(r => r.Offered);
private int TotalAnsweredToday => _dailyStats.Sum(r => r.Answered);
private string AnswerRateToday => TotalCallsToday == 0 ? "—"
    : $"{(TotalAnsweredToday * 100.0 / TotalCallsToday):F1}%";
private string AvgAsaToday
{
    get
    {
        var answered = _dailyStats.Sum(r => r.Answered);
        if (answered == 0) return "—";
        var weightedSum = _dailyStats.Sum(r => (long)r.Asa * r.Answered);
        return $"{weightedSum / answered:F0}s";
    }
}
private int OpenOiCount => _openOi.Count;
private static Color AsaColor(int asa) => asa switch {
    0 => Color.Default,
    < 10 => Color.Success,
    < 20 => Color.Warning,
    _ => Color.Error
};
```

**Step 2: Add the KPI stat row to `Home.razor`**

In `Home.razor`, replace the existing page header block:
```razor
<MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-4">
    <MudText Typo="Typo.h5" Style="flex:1">TOD Operations Dashboard</MudText>
    ...
</MudStack>
```

With:
```razor
<MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-4">
    <MudText Typo="Typo.h5" Style="flex:1">TOD Operations Dashboard</MudText>

    @if (Cache.IsRefreshing)
    {
        <MudProgressCircular Size="Size.Small" Indeterminate="true" Color="Color.Primary" Class="mr-2" />
        <MudText Typo="Typo.caption" Class="mud-text-secondary mr-3">Updating call stats...</MudText>
    }
    else if (Cache.LastRefreshed.HasValue)
    {
        <MudText Typo="Typo.caption" Class="mud-text-secondary mr-2">
            Stats updated @Cache.LastRefreshed.Value.ToString("h:mm tt")
        </MudText>
    }

    <MudIconButton Icon="@Icons.Material.Filled.Refresh"
                   Size="Size.Small"
                   Tooltip="Refresh call stats &amp; schedule"
                   Disabled="Cache.IsRefreshing"
                   OnClick="ManualRefreshAsync" />
</MudStack>

@* ── KPI stat row ────────────────────────────────────────── *@
<MudGrid Spacing="3" Class="mb-4">
    <MudItem xs="6" sm="3">
        <StatCard Value="@TotalCallsToday.ToString("N0")"
                  Label="Calls Today"
                  Icon="@Icons.Material.Filled.Phone"
                  Color="Color.Primary" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@AnswerRateToday"
                  Label="Answer Rate"
                  Icon="@Icons.Material.Filled.CheckCircle"
                  Color="Color.Success" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@AvgAsaToday"
                  Label="Avg ASA Today"
                  Icon="@Icons.Material.Filled.Timer"
                  Color="Color.Info" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@OpenOiCount.ToString()"
                  Label="Open Op Impacts"
                  Icon="@Icons.Material.Filled.Warning"
                  Color="@(OpenOiCount > 0 ? Color.Warning : Color.Default)" />
    </MudItem>
</MudGrid>
```

**Step 3: Build**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add MyApplication/Components/Pages/Home.razor MyApplication/Components/Pages/Home.razor.cs
git commit -m "feat(ui): add KPI stat row to dashboard (calls, answer rate, ASA, open OI)"
```

---

## Task 7: Home Dashboard — Convert Cards to AppCard + ASA Coloring

**Files:**
- Modify: `MyApplication/Components/Pages/Home.razor`

**Step 1: Convert all `MudPaper` cards in `Home.razor` to `AppCard`**

The home page has 5 content cards. Replace each pattern below.

**Card 1 — Today (lines ~39–92):**

Replace:
```razor
<MudPaper Class="pa-3" Style="height:100%">
    <MudText Typo="Typo.h6" Class="mb-2">Today</MudText>
```
With:
```razor
<AppCard Title="Today" TitleIcon="@Icons.Material.Filled.Today" Style="height:100%">
```
And replace the closing `</MudPaper>` with `</AppCard>`.

**Card 2 — Month to Date (lines ~94–147):**

Replace:
```razor
<MudPaper Class="pa-3" Style="height:100%">
    <MudText Typo="Typo.h6" Class="mb-2">Month to Date</MudText>
```
With:
```razor
<AppCard Title="Month to Date" TitleIcon="@Icons.Material.Filled.CalendarMonth" Style="height:100%">
```
And replace the closing `</MudPaper>` with `</AppCard>`.

**Card 3 — Attendance Today (lines ~150–198):**

The attendance section uses `MudPaper Class="pa-0"` wrapping a `MudExpansionPanels`. Replace:
```razor
<MudPaper Class="pa-0" Style="height:100%">
```
With:
```razor
<MudPaper Elevation="3" Class="pa-0" Style="height:100%;border-radius:8px;">
```
(Keep the `MudExpansionPanels` as-is — the expansion panel already has its own header styling.)

**Card 4 — Resourcing Today:** Same treatment as Card 3 (identical structure).

**Card 5 — Upcoming Events (lines ~251–305):**

Replace:
```razor
<MudPaper Class="pa-3" Style="height:100%">
    <MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-2">
        <MudText Typo="Typo.h6" Style="flex:1">Upcoming Events</MudText>
        @if (_canEdit)
        {
            <MudIconButton Icon="@Icons.Material.Filled.Add"
                           Color="Color.Primary"
                           Size="Size.Small"
                           Tooltip="Add Event"
                           OnClick="OpenAddEventDialogAsync" />
        }
    </MudStack>
```
With:
```razor
<AppCard Title="Upcoming Events" TitleIcon="@Icons.Material.Filled.Event" Style="height:100%">
    <Actions>
        @if (_canEdit)
        {
            <MudIconButton Icon="@Icons.Material.Filled.Add"
                           Color="Color.Primary"
                           Size="Size.Small"
                           Tooltip="Add Event"
                           OnClick="OpenAddEventDialogAsync" />
        }
    </Actions>
    <ChildContent>
```
And close with `</ChildContent></AppCard>`.

**Card 6 — Open Operational Impacts (lines ~307–340):**

Replace:
```razor
<MudPaper Class="pa-3" Style="height:100%">
    <MudText Typo="Typo.h6" Class="mb-2">Open Operational Impacts</MudText>
```
With:
```razor
<AppCard Title="Open Operational Impacts" TitleIcon="@Icons.Material.Filled.Warning" TitleColor="Color.Warning" Style="height:100%">
```
And replace closing `</MudPaper>` with `</AppCard>`.

**Step 2: Add `Class="table-modern"` to all `MudSimpleTable` in Home.razor**

Find every `<MudSimpleTable` in `Home.razor` and add `Class="table-modern"` to it. Example:
```razor
<MudSimpleTable Dense="true" Hover="true" Class="table-modern mb-2">
```

**Step 3: Add ASA color chips to the Today and MTD queue tables**

In both queue tables (Today and MTD), replace the plain `<td>@r.Asa</td>` cell with:
```razor
<td>
    <MudChip T="string" Size="Size.Small" Color="@AsaColor(r.Asa)" Variant="Variant.Text" Class="px-1">
        @r.Asa s
    </MudChip>
</td>
```

**Step 4: Build**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add MyApplication/Components/Pages/Home.razor
git commit -m "feat(ui): convert dashboard cards to AppCard, add ASA color chips, modern table styles"
```

---

## Task 8: Employees Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Employees/Index.razor`
- Modify: `MyApplication/Components/Pages/Employees/Schedules.razor`
- Modify: `MyApplication/Components/Pages/Employees/Details.razor`
- Modify: `MyApplication/Components/Pages/Employees/User.razor`
- Modify: `MyApplication/Components/Pages/Employees/EmployeeRoutingAssignment.razor`

**Step 1: Read each file before editing**

Read all 5 files to understand their current structure before making changes.

**Step 2: Apply the standard pattern to each page**

For each page that has `<MudText Typo="Typo.h5" ...>PageTitle</MudText>` as a standalone title, replace with `<SectionHeader Title="PageTitle" />`.

For any page that has `<MudStack Row ... Justify="Justify.SpaceBetween"><MudText Typo.h5>...</MudText><MudButton>...</MudButton></MudStack>`, replace with:
```razor
<SectionHeader Title="...">
    <Actions>
        <MudButton ...>...</MudButton>
    </Actions>
</SectionHeader>
```

For `MudPaper` cards inside pages, replace `<MudPaper Class="pa-3">` → `<MudPaper Elevation="3" Class="pa-4">` (or use `AppCard` with a `Title` if the paper contains a section heading).

For `MudSimpleTable` elements, add `Class="table-modern"`.

**Step 3: Build**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add MyApplication/Components/Pages/Employees/
git commit -m "feat(ui): apply SectionHeader + AppCard pattern to Employees pages"
```

---

## Task 9: ACR Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Acr/Index.razor`
- Modify: `MyApplication/Components/Pages/Acr/New.razor`
- Modify: `MyApplication/Components/Pages/Acr/Edit.razor`
- Modify: `MyApplication/Components/Pages/Acr/Details.razor`

**Step 1: Read all 4 files**

**Step 2: `Acr/Index.razor` — replace outer `MudPaper Outlined` wrapper**

Current structure:
```razor
<MudStack Spacing="2">
    <MudPaper Class="pa-4" Outlined="true">
        <MudStack Row AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
            <MudText Typo="Typo.h5">ACRs</MudText>
            <MudButton ...>New Request</MudButton>
        </MudStack>
        [tabs + filters + grid]
    </MudPaper>
</MudStack>
```

Replace with:
```razor
<SectionHeader Title="ACRs" Subtitle="Allowance Change Requests">
    <Actions>
        <MudButton Variant="Variant.Filled" Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="@(() => Navigation.NavigateTo("/acr/new"))">
            New Request
        </MudButton>
    </Actions>
</SectionHeader>

<AppCard>
    <ChildContent>
        [tabs + divider + filters + divider + grid — unchanged]
    </ChildContent>
</AppCard>
```

**Step 3: Add status chip color helper to ACR Index code-behind (or inline)**

Find where ACR status is rendered as text or a plain chip and update to use semantic colors:

```csharp
private static Color AcrStatusColor(string status) => status switch {
    "Approved" => Color.Success,
    "Denied"   => Color.Error,
    "Pending"  => Color.Warning,
    "Draft"    => Color.Default,
    _          => Color.Default
};
```

In the data grid status column template, replace bare text with:
```razor
<MudChip T="string" Size="Size.Small" Color="@AcrStatusColor(context.Item.Status)"
         Variant="Variant.Text">@context.Item.Status</MudChip>
```
(Read the file first to find the exact column name and context variable used.)

**Step 4: New/Edit/Details — replace MudPaper sections with AppCard**

For each form/detail card that uses `<MudPaper Class="pa-4">` or `<MudPaper Class="pa-3">`, replace with `<AppCard>` (no title if there's no heading, or add `Title` if there is one nearby).

**Step 5: Build**

Run: `dotnet build MyApplication`
Expected: Build succeeded

**Step 6: Commit**

```bash
git add MyApplication/Components/Pages/Acr/
git commit -m "feat(ui): apply SectionHeader + AppCard + status chips to ACR pages"
```

---

## Task 10: Opera Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Opera/Index.razor`
- Modify: `MyApplication/Components/Pages/Opera/Submit.razor`
- Modify: `MyApplication/Components/Pages/Opera/Details.razor`

**Step 1: Read all 3 files**

**Step 2: `Opera/Index.razor` — same pattern as ACR Index**

Replace the outer `MudPaper Outlined` + `MudStack Row` header with `SectionHeader` + `AppCard` wrapper.

Add status chip color helper:
```csharp
private static Color OperaStatusColor(string status) => status switch {
    "Approved" or "Completed" => Color.Success,
    "Denied"   or "Cancelled" => Color.Error,
    "Pending"  or "In Review" => Color.Warning,
    "Draft"                   => Color.Default,
    _                         => Color.Default
};
```
(Adjust the status string values to match what `OperaStatus` actually uses — read the file first.)

**Step 3: Submit/Details — replace MudPaper sections with AppCard**

Same AppCard pattern as ACR.

**Step 4: Build + commit**

```bash
git add MyApplication/Components/Pages/Opera/
git commit -m "feat(ui): apply SectionHeader + AppCard + status chips to Opera pages"
```

---

## Task 11: Training Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Training/Skills/SkillsLookup.razor`
- Modify: `MyApplication/Components/Pages/Training/Skills/AddSkill.razor`
- Modify: `MyApplication/Components/Pages/Training/Certifications/Index.razor`
- Modify: `MyApplication/Components/Pages/Training/Certifications/Details.razor`
- Modify: `MyApplication/Components/Pages/Training/Certifications/Upload.razor`

**Step 1: Read all 5 files**

**Step 2: Apply standard patterns**

- Replace `MudText Typo.h5` standalone titles → `SectionHeader`
- Replace `MudStack Row + MudText h5 + spacer + buttons` → `SectionHeader` with `Actions`
- Replace `MudPaper pa-3/pa-4` content cards → `AppCard` (with `Title` if a heading exists inside)
- Add `Class="table-modern"` to any `MudSimpleTable`

**Step 3: Build + commit**

```bash
git add MyApplication/Components/Pages/Training/
git commit -m "feat(ui): apply SectionHeader + AppCard to Training pages"
```

---

## Task 12: Tools Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Tools/Interval/IntervalSummary.razor`
- Modify: `MyApplication/Components/Pages/Tools/OperationalImpact/OperationalImpact.razor`
- Modify: `MyApplication/Components/Pages/Tools/OperationalImpact/OperationalImpactItem.razor`
- Modify: `MyApplication/Components/Pages/Tools/OstPassdown/OstPassdownDashboard.razor`
- Modify: `MyApplication/Components/Pages/Tools/OstPassdown/NewPassdown.razor`
- Modify: `MyApplication/Components/Pages/Tools/OstPassdown/PassdownDetails.razor`
- Modify: `MyApplication/Components/Pages/Tools/EmailTemplatesPage/EmailTemplatesPage.razor`
- Modify: `MyApplication/Components/Pages/Tools/Proactive/Proactive.razor`

**Step 1: Read each file before editing**

**Step 2: Apply standard patterns to each**

Same replacements: `SectionHeader`, `AppCard`, `Class="table-modern"` on simple tables.

The `_Toolbar.razor` in Tools/Interval/ is a component fragment — apply AppCard if it contains a `MudPaper` wrapper.

**Step 3: Build + commit**

```bash
git add MyApplication/Components/Pages/Tools/
git commit -m "feat(ui): apply SectionHeader + AppCard to Tools pages"
```

---

## Task 13: WFM Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Wfm/BreakTemplatesPage.razor`
- Modify: `MyApplication/Components/Pages/Wfm/StaticBreaksPage.razor`
- Modify: `MyApplication/Components/Pages/Wfm/Aws/BulkAssignment.razor`
- Modify: `MyApplication/Components/Pages/Wfm/Aws/Profiles/Index.razor`
- Modify: `MyApplication/Components/Pages/Wfm/Aws/Profiles/Details.razor`
- Modify: `MyApplication/Components/Pages/Wfm/Aws/Queues/Index.razor`

**Step 1: Read each file**

**Step 2: Apply standard patterns**

Same as previous tasks. Dialog components (`ProfileDialog`, `QueueDialog`, `AddQueueToProfileDialog`) — read them but only apply `AppCard` if they contain a `MudPaper` inside the dialog body (many dialogs already have their own `MudDialogContent` wrapper).

**Step 3: Build + commit**

```bash
git add MyApplication/Components/Pages/Wfm/
git commit -m "feat(ui): apply SectionHeader + AppCard to WFM pages"
```

---

## Task 14: Admin Pages

**Files:**
- Modify: `MyApplication/Components/Pages/Admin/Roles/Index.razor`
- Modify: `MyApplication/Components/Pages/Admin/FeatureFlags/Index.razor`
- Modify: `MyApplication/Components/Pages/Admin/DebugClaims.razor`

**Step 1: Read each file**

**Step 2: Apply standard patterns**

Same replacements as all prior tasks.

**Step 3: Build + commit**

```bash
git add MyApplication/Components/Pages/Admin/
git commit -m "feat(ui): apply SectionHeader + AppCard to Admin pages"
```

---

## Task 15: Release Notes — Add v1.2 Entry

**Files:**
- Modify: `MyApplication/Components/Pages/Updates/Index.razor`

**Step 1: Read the file to find the current v1.1 entry**

**Step 2: Add v1.2 entry at the top of the timeline**

Following the CLAUDE.md release notes convention:

1. Add a new `MudTimelineItem` block **above** the v1.1 entry
2. Move the `Current` chip to the new v1.2 entry
3. Change the v1.1 entry's chip to `Variant="Variant.Outlined"` and `Color="Color.Default"`
4. Change the v1.1 `MudTimelineItem` Color to `Color.Default` and Icon to `Icons.Material.Filled.CheckCircle`

New entry to insert at top:
```razor
<MudTimelineItem Color="Color.Success" Icon="@Icons.Material.Filled.NewReleases" Size="Size.Medium">
    <ItemOpposite>
        <MudText Typo="Typo.caption" Color="Color.Secondary">March 2026</MudText>
    </ItemOpposite>
    <ItemContent>
        <MudPaper Elevation="2" Class="pa-4 mb-4">
            <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2" Class="mb-3">
                <MudText Typo="Typo.h6">v1.2</MudText>
                <MudChip T="string" Size="Size.Small" Color="Color.Success" Variant="Variant.Filled">Current</MudChip>
            </MudStack>

            <MudText Typo="Typo.subtitle2" Class="mb-1">UI Polish</MudText>
            <MudList T="string" Dense="true" Class="mb-3">
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">Switched to Inter font for sharper, more modern typography</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">Added KPI stat row to dashboard — calls, answer rate, ASA, and open operational impacts at a glance</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">New shared components: AppCard, StatCard, and SectionHeader for consistent page layouts</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">Color-coded ASA chips in queue tables (green / amber / red by threshold)</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">Semantic status chips on ACR and Opera request lists</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">TOD branding header and version badge added to the navigation sidebar</MudListItem>
                <MudListItem T="string" Icon="@Icons.Material.Filled.ArrowRight">Consistent card elevation (Elevation=3), 8px border radius, and improved spacing across all pages</MudListItem>
            </MudList>
        </MudPaper>
    </ItemContent>
</MudTimelineItem>
```

**Step 3: Build + commit**

```bash
git add MyApplication/Components/Pages/Updates/Index.razor
git commit -m "feat(ui): add v1.2 release notes entry for UI polish update"
```

---

## Final Verification

After all tasks are complete:

**Step 1: Full build**

Run: `dotnet build MyApplication`
Expected: Build succeeded, 0 compiler errors (CS/RZ errors — MSB file-lock warnings are not errors)

Verify with: `dotnet build MyApplication 2>&1 | grep -E "error CS|error RZ"`
Expected: no output (empty means clean)

**Step 2: Confirm all new components are in place**

```
MyApplication/Components/Shared/AppCard.razor      ✓
MyApplication/Components/Shared/StatCard.razor     ✓
MyApplication/Components/Shared/SectionHeader.razor ✓
```

**Step 3: Run the app and visually verify**

Run: `dotnet run --project MyApplication`
Check:
- [ ] Inter font loads (text looks crisper / different from Roboto)
- [ ] Nav sidebar shows "TOD" branding header and "v1.2" at bottom
- [ ] Dashboard shows 4 KPI stat cards above the main grid
- [ ] Queue tables show colored ASA chips
- [ ] All pages show consistent `SectionHeader` + `AppCard` layouts
- [ ] Dark mode and light mode both look correct (no hardcoded colors breaking themes)

---

## Summary of Files Changed

**New (3 files):**
- `MyApplication/Components/Shared/AppCard.razor`
- `MyApplication/Components/Shared/StatCard.razor`
- `MyApplication/Components/Shared/SectionHeader.razor`

**Modified (~25 files):**
- `MyApplication/Components/App.razor`
- `MyApplication/wwwroot/css/app.css`
- `MyApplication/Components/Layout/NavMenu.razor`
- `MyApplication/Components/Pages/Home.razor`
- `MyApplication/Components/Pages/Home.razor.cs`
- `MyApplication/Components/Pages/Employees/Index.razor`
- `MyApplication/Components/Pages/Employees/Details.razor`
- `MyApplication/Components/Pages/Employees/Schedules.razor`
- `MyApplication/Components/Pages/Employees/User.razor`
- `MyApplication/Components/Pages/Employees/EmployeeRoutingAssignment.razor`
- `MyApplication/Components/Pages/Acr/Index.razor`
- `MyApplication/Components/Pages/Acr/New.razor`
- `MyApplication/Components/Pages/Acr/Edit.razor`
- `MyApplication/Components/Pages/Acr/Details.razor`
- `MyApplication/Components/Pages/Opera/Index.razor`
- `MyApplication/Components/Pages/Opera/Submit.razor`
- `MyApplication/Components/Pages/Opera/Details.razor`
- `MyApplication/Components/Pages/Training/` (5 pages)
- `MyApplication/Components/Pages/Tools/` (8 pages)
- `MyApplication/Components/Pages/Wfm/` (6 pages)
- `MyApplication/Components/Pages/Admin/` (3 pages)
- `MyApplication/Components/Pages/Updates/Index.razor`
