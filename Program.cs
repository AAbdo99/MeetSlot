using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeetSlot.Data;
using MeetSlot.Middleware;
using MeetSlot.Repositories;
using MeetSlot.Repositories.Interfaces;
using MeetSlot.Services;
using MeetSlot.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Database
builder.Services.AddDbContext<MeetSlotDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// TokenService registreres slik at den kan brukes i controllers via dependency injection
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// JWT-autentisering aktiveres kun hvis alle JWT-verdier finnes i konfigurasjon.
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

if (!string.IsNullOrWhiteSpace(jwtIssuer)
    && !string.IsNullOrWhiteSpace(jwtAudience)
    && !string.IsNullOrWhiteSpace(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}
else
{
    // Lar appen starte lokalt uten JWT-secrets (f.eks. for enkel Razor-visning).
    builder.Services.AddAuthentication();
}


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Samler valideringsfeil i ett felles responsformat for hele API-et.
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new
        {
            error = "Validering feilet.",
            status = StatusCodes.Status400BadRequest,
            traceId = context.HttpContext.TraceIdentifier,
            details = errors
        });
    };
});
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware rekkefølge er viktig!
app.UseMiddleware<GlobalExceptionMiddleware>();  // Global error handler først (fanger alt)
app.UseMiddleware<RequestLoggingMiddleware>();  // Logger alle innkommende requests (må komme før auth for å logge også auth-feil, logger request + response, også når det feiler))
app.UseHttpsRedirection();                     // Håndterer 301-redirect fra http til https (må komme før auth for å unngå at http-requests feiler med 401 uten å bli logget)

// Security:Autentisering og autorisasjon må komme før MapControllers for å sikre at alle API-endepunkter er beskyttet (med mindre de har [AllowAnonymous])
app.UseAuthentication(); // Må komme før UseAuthorization! 
app.UseAuthorization();  // Håndterer 403 Forbidden for requests uten gyldig token (må komme etter auth for å fungere)

// Endpoints
app.MapControllers();    // MapControllers må komme etter UseAuthentication og UseAuthorization for å sikre at alle API-endepunkter er beskyttet (med mindre de har [AllowAnonymous])
app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/rooms"));

// For logging skal logge “raw exception før den blir håndtert”, det må legges RequestLogging først.
/// Om logging skal logges “final status code etter exception er gjort om til respons”, Så derfor det må legges GlobalException først.

app.MapGet("/log-test", (ILogger<Program> logger) =>
{
    logger.LogWarning("TEST LOGG: /log-test ble kalt");
    return Results.Ok("Log test kjørt");
});



app.Run();

