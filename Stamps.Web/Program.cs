using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stamps.Shared.Services;
using Stamps.Web.Components;
using Stamps.Web.Data;
using Stamps.Web.Services;
using Stamps.Web.Middleware;
using Serilog;
using AspNetCoreRateLimit;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add environment variables to configuration (for Render, Railway, etc.)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Controllers for API endpoints
builder.Services.AddControllers();

// Add ProblemDetails for standardized error responses
builder.Services.AddProblemDetails();

// Add Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add HttpContextAccessor for audit fields
builder.Services.AddHttpContextAccessor();

// Add distributed cache (required for session)
builder.Services.AddDistributedMemoryCache();

// Add session support (for WebPreferencesService)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add CORS for mobile app
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add device-specific services used by the Stamps.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add Preferences Service for web (uses session storage)
builder.Services.AddSingleton<IPreferencesService, WebPreferencesService>();

// Add Auth State Service (used by Blazor components)
builder.Services.AddSingleton<AuthStateService>();

// Add Entity Framework and PostgreSQL (Supabase)
// Get connection string from configuration (which includes environment variables)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Please set DATABASE_URL environment variable or ConnectionStrings:DefaultConnection in appsettings.json.");

// Handle Supabase connection string format if needed (postgres://user:pass@host:port/db)
if (connectionString.StartsWith("postgres://"))
{
    try
    {
        // Convert Supabase URI format to Npgsql connection string format
        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        var database = uri.LocalPath.TrimStart('/');
        if (string.IsNullOrEmpty(database)) database = "postgres";
        
        connectionString = $"Host={uri.Host};Database={database};Username={Uri.UnescapeDataString(userInfo[0])};Password={Uri.UnescapeDataString(userInfo[1])};Port={uri.Port};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Failed to parse DATABASE_URL: {ex.Message}", ex);
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        
        if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        }
    });

// Add custom services
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<IStampService, StampService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IGeocodeService, GeocodeService>();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Configure Kestrel to use PORT environment variable (for Railway, Render, etc.)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(port));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection(); // Only redirect to HTTPS in production
}

app.UseSerilogRequestLogging();
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseStaticFiles();
app.UseAntiforgery();
app.UseCors();

// Enable session (required for WebPreferencesService)
app.UseSession();

// Use Rate Limiting
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(Stamps.Shared._Imports).Assembly);

app.Run();
