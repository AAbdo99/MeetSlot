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

        public AuthService(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task RegisterAsync(RegisterRequest dto)
        {
            // Sjekker om brukeren allerede finnes, slik at vi unngår duplikate kontoer.
            var eksisterer = await _userRepository.EmailExistsAsync(dto.Email);
            if (eksisterer)
            {
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
        }

        public async Task<string> LoginAsync(LoginRequest dto)
        {
            // Henter bruker basert på e-post før passordet verifiseres.
            var bruker = await _userRepository.GetByEmailAsync(dto.Email);

            // Bevisst vag feilmelding, slik at vi ikke avslører hva som er feil.
            if (bruker == null || !BCrypt.Net.BCrypt.Verify(dto.Password, bruker.PasswordHash))
            {
                // Meldingsnøkkel: ExceptionMessages.Autentisering.UgyldigeInnloggingsdata
                throw new UnauthorizedException(ExceptionMessages.Autentisering.UgyldigeInnloggingsdata);
            }

            // Token lages først når identiteten er verifisert.
            return _tokenService.GenerateToken(bruker);
        }
    }
}
