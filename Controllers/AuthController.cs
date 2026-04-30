using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetSlot.Data;
using MeetSlot.DTO;
using MeetSlot.Models;
using MeetSlot.Services;

namespace MeetSlot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MeetSlotDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(MeetSlotDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST api/auth/register - for brukere som ikke har konto enda.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            // Sjekker om brukeren allerede eksisterer, sånn at det ikke blir mulig å registrere seg dobbelt. 
           var eksisterer = await _context.AppUsers
               .AnyAsync(u => u.Email == dto.Email);

           if (eksisterer)
              return BadRequest("Bruker med denne e-posten finnes allerede.");

           // BCrypt hasher passordet FØR det lagres i databasen.
           // Work factor 12 betyr at hashin tar litt tid og krever litt CPU-kraft.
           var bruker = new AppUser
           {
              Email = dto.Email,
              PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 12),
              Role = UserRole.User

           };

           _context.AppUsers.Add(bruker);
           await _context.SaveChangesAsync();

           return Ok(new { message = "Bruker opprettet" });
        }

        // POST api/auth/login - for brukere som allerede har konto.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            // Sjekker om brukeren eksisterer.
            var bruker = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
                

        /* Bevvist vag tilbakemeldingen om det er epost eller passord feil. 
         Dette for at angripere ikke skal få noen tilbakemelding om hvilken feil de har gjort.
        */
        if (bruker == null || !BCrypt.Net.BCrypt.Verify(dto.Password, bruker.PasswordHash))
            return Unauthorized("Ugyldig e-post eller passord.");

        var token = _tokenService.GenerateToken(bruker);

        return Ok(new { token = token });

        }

    }    

}
   

          