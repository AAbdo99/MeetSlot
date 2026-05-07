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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // POST api/auth/register - for brukere som ikke har konto enda.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            _logger.LogInformation("Register endpoint called. Email={Email}", dto.Email);
            await _authService.RegisterAsync(dto);
            _logger.LogInformation("Register endpoint succeeded. Email={Email}", dto.Email);

            // REST-konvensjon: POST som oppretter ressurs skal returnere 201 Created med Location-header.
            // Created() setter automatisk statuskode til 201 og Content-Type til application/json.
            return Created(string.Empty, new { message = "Bruker opprettet" });
        }

        // POST api/auth/login - for brukere som allerede har konto.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            _logger.LogInformation("Login endpoint called. Email={Email}", dto.Email);
            var token = await _authService.LoginAsync(dto);
            _logger.LogInformation("Login endpoint succeeded. Email={Email}", dto.Email);

            return Ok(new { token });
        }
    }
}
