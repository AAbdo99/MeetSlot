using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeetSlot.Data;
using MeetSlot.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<MeetSlotDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// TokenService registreres slik at den kan brukes i controllers via dependency injection
builder.Services.AddScoped<TokenService>();

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
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Må komme før UseAuthorization!
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/rooms"));

app.Run();