using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Identity.Abstractions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using MudBlazor.Services;
using MyApplication;
using MyApplication.Common.Time;
using MyApplication.Components;
using MyApplication.Components.Data;
using MyApplication.Components.Service;
using MyApplication.Components.Service.Acr;
using MyApplication.Components.Service.Employee;
using MyApplication.Components.Service.Tools.OstPassdown;
using MyApplication.Components.Service.Training;
using MyApplication.Components.Service.Training.Certifications;
using MyApplication.Components.Service.Wfm;
using MyApplication.Components.Service.Email;
using MyApplication.Components.Service.Home;
using MediatR;
using MyApplication.Components.Service.FeatureFlags;
using System.Configuration;

// Load .env file in development (for local testing)
// In Azure, use Application Settings instead
if (File.Exists(".env"))
{
    DotNetEnv.Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------------
// 1. Host Configuration (Windows Service)
// --------------------------------------------------------------------------------
var isWindowsService = WindowsServiceHelpers.IsWindowsService();

if (isWindowsService)
{
    builder.Host.UseWindowsService();
    builder.Logging.AddEventLog();

    // Set content root if running as service
    builder.Configuration.SetBasePath(AppContext.BaseDirectory);
}

// --------------------------------------------------------------------------------
// 2. Database Configuration
// --------------------------------------------------------------------------------

// AOM Context - Using Factory pattern for Blazor Server concurrency
builder.Services.AddDbContextFactory<AomDbContext>((sp, opts) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    opts.UseSqlServer(cfg.GetConnectionString("AOM"), sql => sql.EnableRetryOnFailure());
   // opts.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    opts.EnableSensitiveDataLogging(false); // Disable in prod
});

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMemoryCache(); // Used by ClaimsEnrichmentMiddleware and FeatureFlagService

// --------------------------------------------------------------------------------
// 3. Authentication & Authorization (Entra ID)
// --------------------------------------------------------------------------------

builder.Services.AddHttpContextAccessor();

// Distributed SQL Server token cache — persists MSAL tokens across restarts/deploys,
// preventing the user_null MsalUiRequiredException that occurs when in-memory cache is lost.
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("AOM");
    options.SchemaName = "dbo";
    options.TableName = "MsalTokenCache";
});

// Add Entra ID (Azure AD) authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read", "GroupMember.Read.All", "Mail.Send", "Mail.ReadWrite" })
    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
    .AddDistributedTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // Define policies for pages that use [Authorize(Policy = "Admin")]
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

    // Define policies for pages that use [Authorize(Policy = "OST")]
    options.AddPolicy("OST", policy => policy.RequireRole("Admin", "OST"));
});

// Claims enrichment will be done via middleware instead of IClaimsTransformation
// This allows Graph API calls to work after authentication is complete
// builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, MyApplication.Components.Service.EntraIdClaimsEnricher>();

// Add Razor Pages for Microsoft Identity UI (sign-in/sign-out)
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


// --------------------------------------------------------------------------------
// 4. Core & UI Services
// --------------------------------------------------------------------------------

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddMudServices();

// --------------------------------------------------------------------------------
// 5. Domain Services
// --------------------------------------------------------------------------------

// Common / Helpers
builder.Services.AddScoped<ITimeDisplayService, TimeDisplayService>();
builder.Services.AddScoped<IEmailComposer, EmailComposer>();
builder.Services.AddScoped<UserProfileService>();

// Employee & HR
builder.Services.AddScoped<EmployeeDetailsService>();
builder.Services.AddScoped<EmployeeListService>();
builder.Services.AddScoped<EmployeeScheduleService>();
builder.Services.AddScoped<ISkillsService, SkillsService>(); // Standardized naming

// ACR (Allowance Change Request)
builder.Services.AddScoped<IAcrService, AcrService>();
builder.Services.AddScoped<IAcrQueryService, AcrQueryService>();

// Opera
builder.Services.AddScoped<IOperaRepository, OperaRepository>();

// WFM (Workforce Management) - NEW
builder.Services.AddScoped<IWfmService, WfmService>();

// Tools
builder.Services.AddScoped<IIntervalSummaryRepository, IntervalSummaryRepository>();
builder.Services.AddScoped<IntervalEmailService>();

builder.Services.AddScoped<IOiLookupRepository, OiLookupRepository>();
builder.Services.AddScoped<IOiEventRepository, OiEventRepository>();
builder.Services.AddScoped<OperationalImpactEmailService>();

builder.Services.AddScoped<IProactiveRepository, ProactiveRepository>();
builder.Services.AddScoped<ICertificationsRepository, CertificationsRepository>();

// Home Dashboard
builder.Services.AddSingleton<HomeDashboardCache>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();


builder.Services.AddScoped<MyApplication.Components.Service.Security.SecurityService>();

builder.Services.AddScoped<MyApplication.Components.Service.Aws.AwsRoutingService>();
builder.Services.AddTransient<IOstPassdownService, OstPassdownService>(); // Added



// --------------------------------------------------------------------------------
// 6. Application Pipeline
// --------------------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// Enrich claims with roles from Graph API groups and database
// MUST run after UseAuthentication (so token is ready) but before UseAuthorization (so roles are available)
app.UseMiddleware<ClaimsEnrichmentMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorPages(); // Required for Microsoft Identity UI (sign-in/sign-out)
app.MapGet("/healthz", () => Results.Ok("ok")).AllowAnonymous();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

