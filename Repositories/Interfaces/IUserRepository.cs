using MeetSlot.Models;

namespace MeetSlot.Repositories.Interfaces
{
    // Kontrakt for dataoperasjoner relatert til brukere.
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByIdAsync(int id);
        Task AddAsync(AppUser user);
    }
}
