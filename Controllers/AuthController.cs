using Microsoft.AspNetCore.Mvc;
using MeetSlot.Dtos;
using MeetSlot.Services.Interfaces;

namespace MeetSlot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register - for brukere som ikke har konto enda.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            await _authService.RegisterAsync(dto);

            // REST-konvensjon: POST som oppretter ressurs skal returnere 201 Created med Location-header.
            // Created() setter automatisk statuskode til 201 og Content-Type til application/json.
            return Created(string.Empty, new { message = "Bruker opprettet" });
        }

        // POST api/auth/login - for brukere som allerede har konto.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var token = await _authService.LoginAsync(dto);

            return Ok(new { token });
        }
    }
}
