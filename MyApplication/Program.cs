using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting.WindowsServices;
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
using MyApplication.Components.Services.Email;
using System.Configuration;

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

// AWS Context - Standard Scoped
builder.Services.AddDbContext<AwsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AWS"),
        sql => sql.EnableRetryOnFailure()));

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --------------------------------------------------------------------------------
// 3. Authentication & Authorization (Temporary Azure-safe mode)
// --------------------------------------------------------------------------------

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();


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
builder.Services.AddScoped<EmployeesRepository>();
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

app.UseAuthorization();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapControllers();
app.MapGet("/healthz", () => Results.Ok("ok")).AllowAnonymous();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

