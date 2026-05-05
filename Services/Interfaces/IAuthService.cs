using MeetSlot.Dtos;

namespace MeetSlot.Services.Interfaces
{
    // Kontrakt for autentiseringslogikk mellom controller og data-lag.
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest dto); // Registrerer en ny bruker.
        Task<string> LoginAsync(LoginRequest dto); // Verifiserer bruker og returnerer JWT-token ved suksess.
    }
}
