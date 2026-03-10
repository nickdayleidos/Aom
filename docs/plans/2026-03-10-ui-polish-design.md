# UI Polish Design — TOD / AOM
**Date:** 2026-03-10
**Status:** Approved
**Scope:** Full treatment — global CSS foundation + shared Blazor components + dashboard KPI cards + all data pages

---

## Goals

Transform the app from "out-of-the-box MudBlazor" to a polished, modern SaaS-style internal ops tool while:
- Keeping the existing purple color scheme (`#6c5ffc` / `#7e6fff`)
- Preserving all existing functionality and data flows
- Staying within MudBlazor's component model (no third-party UI libraries)

---

## Section 1 — Global Foundation

### Typography
- Replace Roboto with **Inter** (Google Fonts)
- Add `<link>` to `MyApplication/Components/App.razor` (or `_Host.cshtml` if present)
- Override `body { font-family: 'Inter', sans-serif; }` in `app.css`

### Border Radius
- Add `--app-radius: 8px` CSS variable
- Override MudBlazor paper/card radius: `.mud-paper { border-radius: var(--app-radius) !important; }`

### Elevation System
| Level | Elevation | Used for |
|-------|-----------|----------|
| 0 | Flat | Table rows, backgrounds |
| 3 | Standard | Content cards (`AppCard`) |
| 8 | Raised | Dialogs, modals |

Update `MudDialogProvider` to use `Elevation="8"` by default (via CSS override or provider param).

### Spacing
- `MudMainContent` class: `pt-16 pa-6` (was `pa-4`)
- `AppCard` internal padding: `pa-4` (was `pa-3`)

---

## Section 2 — Shared Blazor Components

### Files to create
All in `MyApplication/Components/Shared/`:

#### `AppCard.razor`
Replaces inline `MudPaper` + `MudText h6` header pattern used on every page.

**Parameters:**
- `Title` (string) — card title, rendered as `Typo.subtitle1` with `font-weight: 600`
- `TitleIcon` (string, optional) — MudBlazor icon string shown left of title
- `TitleColor` (Color, optional, default `Color.Default`) — icon color
- `Actions` (RenderFragment, optional) — right side of header row (buttons, chips)
- `ChildContent` (RenderFragment) — card body

**Renders:**
```
MudPaper (Elevation=3, Class="pa-4")
  MudStack Row (header, if Title set)
    [Icon] Title text         Actions slot
  ChildContent
```

#### `StatCard.razor`
KPI summary card for the dashboard.

**Parameters:**
- `Value` (string) — the metric (e.g. "1,842", "94.2%")
- `Label` (string) — descriptor below (e.g. "Calls Today")
- `Icon` (string) — MudBlazor icon
- `Color` (Color) — icon/accent color
- `Subtitle` (string, optional) — secondary line (e.g. "↑ 12% vs yesterday")

**Renders:**
```
MudPaper (Elevation=3, Class="pa-4")
  MudStack Row AlignItems.Center
    MudAvatar (Color, Variant.Rounded, Size.Large)
      MudIcon (Icon)
    MudStack
      MudText Typo.h5 (Value, font-weight 700)
      MudText Typo.body2 color secondary (Label)
      [MudText caption (Subtitle) if set]
```

#### `SectionHeader.razor`
Replaces the ad-hoc `MudStack Row + MudText h5 + MudSpacer + buttons` pattern on every page.

**Parameters:**
- `Title` (string)
- `Subtitle` (string, optional)
- `Actions` (RenderFragment, optional)

**Renders:**
```
MudStack Class="mb-4"
  MudStack Row AlignItems.Center
    MudStack (grow)
      MudText Typo.h5 (Title)
      [MudText Typo.caption secondary (Subtitle) if set]
    [Actions slot]
```

---

## Section 3 — Navigation

### File: `NavMenu.razor`

Add a branding block at the top (above `<MudNavMenu>`):
```razor
<div class="nav-brand">
    <span class="nav-brand-logo">TOD</span>
    <span class="nav-brand-sub">Total Operations Database</span>
</div>
```

Add version badge at the bottom (below last `MudDivider`):
```razor
<div class="nav-version">v1.1</div>
```

### CSS additions to `app.css`
```css
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
    letter-spacing: 0.08em;
    margin-top: 2px;
}
.nav-version {
    padding: 8px 16px;
    font-size: 0.7rem;
    color: var(--mud-palette-text-disabled);
}
```

---

## Section 4 — Home Dashboard

### File: `Home.razor`

**KPI stat row** — add above the `<MudGrid>`:
```razor
<MudGrid Spacing="3" Class="mb-4">
    <MudItem xs="6" sm="3">
        <StatCard Value="@_totalCalls.ToString("N0")" Label="Calls Today"
                  Icon="@Icons.Material.Filled.Phone" Color="Color.Primary" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@_answerRate" Label="Answer Rate"
                  Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Success" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@_avgAsa" Label="Avg ASA"
                  Icon="@Icons.Material.Filled.Timer" Color="Color.Info" />
    </MudItem>
    <MudItem xs="6" sm="3">
        <StatCard Value="@_openOi.Count.ToString()" Label="Open OI"
                  Icon="@Icons.Material.Filled.Warning"
                  Color="@(_openOi.Count > 0 ? Color.Warning : Color.Default)" />
    </MudItem>
</MudGrid>
```

Computed properties (in `Home.razor.cs`) from existing `_dailyStats`:
```csharp
private int _totalCalls => _dailyStats.Sum(r => r.Offered);
private string _answerRate => _totalCalls == 0 ? "—"
    : $"{(_dailyStats.Sum(r => r.Answered) * 100.0 / _totalCalls):F1}%";
private string _avgAsa => _dailyStats.Count == 0 ? "—"
    : $"{_dailyStats.Average(r => r.Asa):F0}s";
```

**Replace all `MudPaper` → `AppCard`** with appropriate `Title` and `TitleIcon`.

**Color-coded ASA in queue tables** — wrap ASA cell:
```razor
<MudChip T="string" Size="Size.Small"
         Color="@GetAsaColor(r.Asa)"
         Variant="Variant.Text">@r.Asa s</MudChip>
```
Helper:
```csharp
private static Color GetAsaColor(int asa) => asa switch {
    < 10 => Color.Success,
    < 20 => Color.Warning,
    _    => Color.Error
};
```

---

## Section 5 — All Data Pages

Apply to every page across: Employees, ACR, Opera, Training, Tools, WFM, Admin.

### Pattern replacements

| Old pattern | New pattern |
|------------|-------------|
| `<MudPaper Class="pa-3">` + `<MudText Typo.h6">` | `<AppCard Title="...">` |
| `<MudStack Row ... Class="mb-4">` + `<MudText Typo.h5">` + `<MudSpacer/>` + buttons | `<SectionHeader Title="..." ...><Actions>buttons</Actions></SectionHeader>` |
| Bare `Variant="Variant.Filled"` status chips | Colored `MudChip` with semantic `Color` |
| `MudSimpleTable` (where upgradeable) | Add `Class="table-modern"` CSS class |

### Table CSS (add to `app.css`)
```css
.table-modern th {
    font-weight: 600;
    font-size: 0.75rem;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    color: var(--mud-palette-text-secondary);
    padding: 10px 16px;
}
.table-modern td {
    padding: 10px 16px;
    border-bottom: 1px solid var(--mud-palette-divider);
}
.table-modern tbody tr:last-child td {
    border-bottom: none;
}
```

### Status chip colors
Define a shared convention:
- ACR Status: `Pending` → Warning, `Approved` → Success, `Denied` → Error, `Draft` → Default
- Opera Status: same pattern
- Employee active/inactive: Success / Default

---

## Implementation Order

1. **Global CSS + typography** (`app.css`, `App.razor`) — touches 2 files, benefits everything immediately
2. **Shared components** (`AppCard`, `StatCard`, `SectionHeader`) — create 3 new files
3. **NavMenu** — branding header + version badge
4. **Home dashboard** — KPI cards + convert to AppCard + ASA coloring
5. **Employees pages** — SectionHeader + AppCard + status chips
6. **ACR pages** — SectionHeader + AppCard + status chips
7. **Opera pages** — same
8. **Training pages** — same
9. **Tools pages** — same
10. **WFM pages** — same
11. **Admin pages** — same
12. **Release notes** — add v1.2 entry to `Updates/Index.razor`

---

## Files Created/Modified (summary)

**New files (3):**
- `MyApplication/Components/Shared/AppCard.razor`
- `MyApplication/Components/Shared/StatCard.razor`
- `MyApplication/Components/Shared/SectionHeader.razor`

**Modified files:**
- `MyApplication/wwwroot/css/app.css` — typography, radius, table styles, nav brand CSS
- `MyApplication/Components/App.razor` — Inter font link
- `MyApplication/Components/Layout/MainLayout.razor` — elevation adjustments
- `MyApplication/Components/Layout/NavMenu.razor` — branding header + version
- `MyApplication/Components/Pages/Home.razor` — KPI cards, AppCard, ASA colors
- `MyApplication/Components/Pages/Home.razor.cs` — computed KPI properties
- All page `.razor` files across Employees, ACR, Opera, Training, Tools, WFM, Admin
- `MyApplication/Components/Pages/Updates/Index.razor` — v1.2 release notes entry
