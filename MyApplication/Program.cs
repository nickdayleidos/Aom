using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using MudBlazor.Services;

using MyApplication;
using MyApplication.Components;
using MyApplication.Components.Data;
using MyApplication.Components.Service;
using MyApplication.Components.Services.Email;

// ────────────────────────────────────────────────────────────────────────────────
// Hosting & Builder
// ────────────────────────────────────────────────────────────────────────────────

var isWindowsService = WindowsServiceHelpers.IsWindowsService();

// When running as a Windows Service, set ContentRoot to the folder containing the .exe/.dll
var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = isWindowsService ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

// Windows Service hosting (no effect under IIS or `dotnet run`)
if (isWindowsService)
{
    builder.Host.UseWindowsService();
    builder.Logging.AddEventLog();

    // Bind URLs when running as service (can also use ASPNETCORE_URLS env var)
    builder.WebHost.UseKestrel()
                   .UseUrls("https://0.0.0.0:8443");
}

// ────────────────────────────────────────────────────────────────────────────────
// Services
// ────────────────────────────────────────────────────────────────────────────────

// AuthN/AuthZ
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(); // Windows auth (Kerberos/NTLM)

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OST", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\SMIT_TODAdmin") ||
            ctx.User.IsInRole(@"LEIDOS-CORP\SMIT_OST")));

    options.AddPolicy("PFM", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\sftp_SMIT_PFRM") ||
            ctx.User.IsInRole(@"YOURDOMAIN\IT Admin")));

    // Example “Admin” policy (single user)
    options.AddPolicy("Admin", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.Identity?.Name?.Equals(
                @"LEIDOS-CORP\dayng",
                StringComparison.OrdinalIgnoreCase) == true));

    // Require authentication by default
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddScoped<IIntervalSummaryRepository, IntervalSummaryRepository>();


builder.Services.AddHttpClient();

// Razor Components (.NET 9)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Optional Web API controllers (keep if you have any)
builder.Services.AddControllers();

// MudBlazor UI services
builder.Services.AddMudServices();

// DbContexts
builder.Services.AddDbContext<AomDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AOM")));

builder.Services.AddDbContext<AwsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AWS")));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Program.cs
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IAwsIntervalReader, AwsIntervalReader>();
builder.Services.AddScoped<IIntervalSummaryRepository, IntervalSummaryRepository>(); // writes





// App services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<
    Microsoft.AspNetCore.Authentication.IClaimsTransformation,
    WindowsClaimsEnricher>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<IEmailComposer, EmailComposer>();
builder.Services.AddScoped<IntervalEmailService>();
builder.Services.AddScoped<IOiLookupRepository, OiLookupRepository>();
builder.Services.AddScoped<IOiEventRepository, OiEventRepository>();
builder.Services.AddScoped<OperationalImpactEmailService>();
builder.Services.AddScoped<IProactiveRepository, ProactiveRepository>();
builder.Services.AddScoped<IOperaRepository, OperaRepository>();





// Needed for short-lived desktop-compose token exchange
// ────────────────────────────────────────────────────────────────────────────────
// Build
// ────────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ────────────────────────────────────────────────────────────────────────────────
/* Pipeline */
// ────────────────────────────────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseMigrationsEndPoint(); // keep if you rely on it in prod
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets(); // serves wwwroot/static web assets for components
app.UseAntiforgery();  // required for forms in Razor Components

app.MapControllers();  // only if you actually have controllers

// ────────────────────────────────────────────────────────────────────────────────
// Minimal APIs: Desktop Outlook compose helper
//   - POST receives a draft and returns a short token (valid 2 minutes)
//   - GET is used by the local helper EXE to fetch the draft by token
//   - Both endpoints allow anonymous to work outside of Windows-integrated auth
// ────────────────────────────────────────────────────────────────────────────────


// ────────────────────────────────────────────────────────────────────────────────
// Razor Components entry point
// ────────────────────────────────────────────────────────────────────────────────

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
