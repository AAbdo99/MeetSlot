using MeetSlot.Data;
using MeetSlot.Models;
using MeetSlot.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetSlot.Repositories
{
    // Implementasjon av IUserRepository med EF Core/DbContext.
    public class UserRepository : IUserRepository
    {
        private readonly MeetSlotDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            MeetSlotDbContext context,
            ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            _logger.LogDebug("Repo EmailExistsAsync. Email={Email}", email);
            return _context.AppUsers.AnyAsync(u => u.Email == email);
        }

        public Task<AppUser?> GetByEmailAsync(string email)
        {
            _logger.LogDebug("Repo GetByEmailAsync. Email={Email}", email);
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<AppUser?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Repo GetByIdAsync. UserId={UserId}", id);
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(AppUser user)
        {
            try
            {
                // Repositoriet håndterer persistering, slik at service-laget er databasenøytralt.
                _logger.LogDebug("Repo AddAsync. Email={Email}, Role={Role}", user.Email, user.Role);
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while adding user. Email={Email}", user.Email);
                throw;
            }
        }
    }
}
