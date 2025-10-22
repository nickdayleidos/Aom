using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using MudBlazor.Services;
using MyApplication;
using MyApplication.Components;
using MyApplication.Components.Data;
using MyApplication.Components.Service;
using MyApplication.Components.Service.Acr;
using MyApplication.Components.Services.Email;
using System.Configuration;

// ────────────────────────────────────────────────────────────────────────────────
// Hosting & Builder
// ────────────────────────────────────────────────────────────────────────────────

var isWindowsService = WindowsServiceHelpers.IsWindowsService();

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = isWindowsService ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

if (isWindowsService)
{
    builder.Host.UseWindowsService();
    builder.Logging.AddEventLog();
    builder.WebHost.UseKestrel().UseUrls("https://0.0.0.0:8443");
}

// ────────────────────────────────────────────────────────────────────────────────
// Services
// ────────────────────────────────────────────────────────────────────────────────

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

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
    options.AddPolicy("WFM", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\sftp_SMIT_PFRM") ||
            ctx.User.IsInRole(@"YOURDOMAIN\IT Admin")));
    options.AddPolicy("C&C", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\sftp_SMIT_PFRM") ||
            ctx.User.IsInRole(@"YOURDOMAIN\IT Admin")));
    options.AddPolicy("Supervisor", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\sftp_SMIT_PFRM") ||
            ctx.User.IsInRole(@"YOURDOMAIN\IT Admin")));
    options.AddPolicy("Manager", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\sftp_SMIT_PFRM") ||
            ctx.User.IsInRole(@"YOURDOMAIN\IT Admin")));

    options.AddPolicy("Admin", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(@"LEIDOS-CORP\SMIT_TODAdmin") ||
            string.Equals(ctx.User.Identity?.Name, @"LEIDOS-CORP\dayng", StringComparison.OrdinalIgnoreCase)));

    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddMudServices();

// ────────────────────────────────────────────────────────────────────────────────
// Data access
// ────────────────────────────────────────────────────────────────────────────────

// ✅ Use ONLY the factory for AomDbContext (no AddDbContext for AOM)
builder.Services.AddDbContextFactory<AomDbContext>((sp, opts) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    opts.UseSqlServer(cfg.GetConnectionString("AOM"),
        sql => sql.EnableRetryOnFailure());
    opts.EnableSensitiveDataLogging(false);
});

// Keep AWS as a normal scoped DbContext
builder.Services.AddDbContext<AwsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AWS"),
        sql => sql.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Other app/data services
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IIntervalSummaryRepository, IntervalSummaryRepository>();

builder.Services.AddScoped<
    Microsoft.AspNetCore.Authentication.IClaimsTransformation,
    WindowsClaimsEnricher>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<IAcrService, AcrService>();
builder.Services.AddScoped<IAcrQueryService, AcrQueryService>();
builder.Services.AddScoped<IEmailComposer, EmailComposer>();
builder.Services.AddScoped<IntervalEmailService>();
builder.Services.AddScoped<IOiLookupRepository, OiLookupRepository>();
builder.Services.AddScoped<IOiEventRepository, OiEventRepository>();
builder.Services.AddScoped<OperationalImpactEmailService>();
builder.Services.AddScoped<IProactiveRepository, ProactiveRepository>();
builder.Services.AddScoped<IOperaRepository, OperaRepository>();

// ────────────────────────────────────────────────────────────────────────────────
// Build & Pipeline
// ────────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
