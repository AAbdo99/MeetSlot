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

        public UserRepository(MeetSlotDbContext context)
        {
            _context = context;
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.AppUsers.AnyAsync(u => u.Email == email);
        }

        public Task<AppUser?> GetByEmailAsync(string email)
        {
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<AppUser?> GetByIdAsync(int id)
        {
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(AppUser user)
        {
            // Repositoriet håndterer persistering, slik at service-laget er databasenøytralt.
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
