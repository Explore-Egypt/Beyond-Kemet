using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ExploreEgypt.Infrastructure;
using ExploreEgypt.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            RequireExpirationTime = true,

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdValue = context.Principal?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                var tokenStamp = context.Principal?
                    .FindFirst("security_stamp")?.Value;

                if (!Guid.TryParse(userIdValue, out var userId) ||
                    string.IsNullOrWhiteSpace(tokenStamp))
                {
                    context.Fail("Invalid or revoked access token.");
                    return;
                }

                var userManager = context.HttpContext.RequestServices
                    .GetRequiredService<UserManager<ApplicationUser>>();

                var user = await userManager.FindByIdAsync(
                    userId.ToString());

                if (user is null ||
                    !user.IsActive ||
                    string.IsNullOrWhiteSpace(user.SecurityStamp) ||
                    !string.Equals(
                        user.SecurityStamp,
                        tokenStamp,
                        StringComparison.Ordinal))
                {
                    context.Fail("Invalid or revoked access token.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Secure by default: endpoints require authentication unless
    // explicitly marked with [AllowAnonymous].
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy(
        "AdminOnly",
        policy => policy.RequireRole("Admin"));

    options.AddPolicy(
        "CompanyOnly",
        policy => policy.RequireRole("Company"));

    options.AddPolicy(
        "TravelerOnly",
        policy => policy.RequireRole("Traveler"));
});

var app = builder.Build();

await IdentityRoleSeeder.SeedRolesAsync(
    app.Services,
    app.Configuration,
    app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();