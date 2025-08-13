using System.Text;
using System.Text.Json.Serialization;
using FinTrack.Api.Background;
using FinTrack.Api.Background.Filters;
using FinTrack.Api.Middleware;
using FinTrack.BusinessLogic;
using FinTrack.BusinessLogic.Services.Auth;
using FinTrack.DataAccess;
using FinTrack.Shared.Config;
using FinTrack.Shared.Entities.Auth;
using FinTrack.Shared.Exceptions;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var apiConfig = builder.Configuration.GetSection("ApiConfig").Get<ApiConfig>();
if (apiConfig == null)
{
    throw new BaseException("Config is corrupt");
}
var hangfireConfig = builder.Configuration.GetSection("Hangfire").Get<HangfireConfig>();
if (hangfireConfig == null)
{
    throw new Exception("Error - incorrect hangfire config - unable to map hangfire config");
}

// Add services to the container.
builder.Services.AddSingleton<ApiConfig>(apiConfig);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services
    .AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddBusinessLogic(builder.Configuration);
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => { 
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<FinDbContext>()
    .AddDefaultTokenProviders();

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

authenticationBuilder.AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.RequireHttpsMetadata = false;
    var jwtSecret = apiConfig.JWTConfig.Secret;
    if (string.IsNullOrWhiteSpace(jwtSecret))
    {
        throw new BaseException("Invalid JWT config");
    }
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = apiConfig.JWTConfig.ValidIssuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});
//if (apiConfig.Authentication.Google.Enabled) {
//    authenticationBuilder.AddGoogle(options =>
//    {
//        options.ClientId = apiConfig.Authentication.Google.ClientId;
//        options.ClientSecret = apiConfig.Authentication.Google.ClientSecret;
//    });
//}

// Hangfire
// Register Hangfire with in-memory storage:
builder.Services.AddHangfire(config =>
{
    config.UseInMemoryStorage();
});

// Optionally: configure Hangfire Server
builder.Services.AddHangfireServer();
builder.Services.AddJobs();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();
app.UseHttpsRedirection();



app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = [new HangfireDashboardAuthFilter(hangfireConfig.Username, hangfireConfig.Password)]
});

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<FinDbContext>();
    if (dbContext == null)
    {
        throw new BaseException("Cannot initialize dbContext");
    }
    await dbContext.Database.MigrateAsync();
}

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var authService = scope.ServiceProvider.GetService<IAuthService>();
    if (authService == null)
    {
        throw new Exception("Cannot initialize authService");
    }

    authService.EnsureRolesExistInDb().Wait();
    authService.EnsureAdminExists().Wait();
}

BackgroundJobsManager.RegisterJobs();

app.Run();