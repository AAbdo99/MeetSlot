using MeetSlot.Data;
using MeetSlot.Models;
using MeetSlot.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetSlot.Repositories
{
    // Implementasjon av IBookingRepository med EF Core/DbContext.
    public class BookingRepository : IBookingRepository
    {
        private readonly MeetSlotDbContext _context;

        public BookingRepository(MeetSlotDbContext context)
        {
            _context = context;
        }

        public Task<Booking?> GetByIdAsync(int id)
        {
            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<Booking?> GetByIdForUserAsync(int id, int userId)
        {
            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.AppUserId == userId);
        }

        public Task<List<Booking>> ListAsync()
        {
            return _context.Bookings.ToListAsync();
        }

        public Task<List<Booking>> ListByUserAsync(int userId)
        {
            return _context.Bookings
                .Where(b => b.AppUserId == userId)
                .ToListAsync();
        }

        public Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end)
        {
            // Overlapp-sjekk for å hindre dobbeltbooking av samme rom.
            return _context.Bookings.AnyAsync(b =>
                b.MeetingRoomId == roomId &&
                start < b.EndTime &&
                end > b.StartTime);
        }

        public Task<List<Booking>> GetForRoomOnDayAsync(int roomId, DateTime dayStart, DateTime dayEnd)
        {
            return _context.Bookings
                .Where(b => b.MeetingRoomId == roomId &&
                            b.StartTime < dayEnd &&
                            b.EndTime > dayStart)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public Task<MeetingRoom?> GetMeetingRoomByIdAsync(int roomId)
        {
            return _context.MeetingRooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public Task<AppUser?> GetUserByIdAsync(int userId)
        {
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
