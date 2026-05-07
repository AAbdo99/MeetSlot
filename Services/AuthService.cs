using MeetSlot.Dtos;
using MeetSlot.Exceptions;
using MeetSlot.Models;
using MeetSlot.Repositories.Interfaces;
using MeetSlot.Services.Interfaces;

namespace MeetSlot.Services
{
    // Samler auth-regler (register/login) i ett service-lag.
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            TokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task RegisterAsync(RegisterRequest dto)
        {
            _logger.LogInformation("Register requested. Email={Email}", dto.Email);

            // Sjekker om brukeren allerede finnes, slik at vi unngår duplikate kontoer.
            var eksisterer = await _userRepository.EmailExistsAsync(dto.Email);
            if (eksisterer)
            {
                _logger.LogWarning("Register rejected. Email already exists. Email={Email}", dto.Email);

                // Meldingsnøkkel: ExceptionMessages.Autentisering.EpostAlleredeRegistrert
                throw new ConflictException(ExceptionMessages.Autentisering.EpostAlleredeRegistrert);
            }

            // BCrypt hasher passordet før lagring, slik at vi aldri lagrer råtekst-passord.
            var bruker = new AppUser
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 12),
                Role = UserRole.User
            };

            await _userRepository.AddAsync(bruker);
            _logger.LogInformation(
                "Register success. UserId={UserId}, Email={Email}, Role={Role}",
                bruker.Id,
                bruker.Email,
                bruker.Role);
        }

        public async Task<string> LoginAsync(LoginRequest dto)
        {
            _logger.LogInformation("Login attempt. Email={Email}", dto.Email);

            // Henter bruker basert på e-post før passordet verifiseres.
            var bruker = await _userRepository.GetByEmailAsync(dto.Email);

            // Bevisst vag feilmelding, slik at vi ikke avslører hva som er feil.
            if (bruker == null || !BCrypt.Net.BCrypt.Verify(dto.Password, bruker.PasswordHash))
            {
                _logger.LogWarning("Login failed. Email={Email}", dto.Email);

                // Meldingsnøkkel: ExceptionMessages.Autentisering.UgyldigeInnloggingsdata
                throw new UnauthorizedException(ExceptionMessages.Autentisering.UgyldigeInnloggingsdata);
            }

            // Token lages først når identiteten er verifisert.
            var token = _tokenService.GenerateToken(bruker);
            _logger.LogInformation(
                "Login success. UserId={UserId}, Email={Email}, Role={Role}",
                bruker.Id,
                bruker.Email,
                bruker.Role);

            return token;
        }
    }
}
