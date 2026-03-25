using Microsoft.EntityFrameworkCore;
using MeetSlot.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext henter aktiv connection string fra config/user-secrets.
builder.Services.AddDbContext<MeetSlotDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers brukes videre når API-endepunktene kommer på plass.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger er kun aktiv i development for enklere lokal testing.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Authorization blir brukt når auth-delen kobles inn av resten av teamet.
app.UseAuthorization();
app.MapControllers();

app.Run();