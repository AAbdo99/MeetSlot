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
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(MeetSlotDbContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<Booking?> GetByIdAsync(int id)
        {
            _logger.LogInformation(
                "Repository: Henter booking {BookingId}",
                id);

            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<Booking?> GetByIdForUserAsync(int id, int userId)
        {
            _logger.LogInformation(
                "Repository: Henter booking {BookingId} for bruker {UserId}",
                id,
                userId);

            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.AppUserId == userId);
        }

        public Task<List<Booking>> ListAsync()
        {
            _logger.LogInformation(
                "Repository: Henter alle bookinger");

            return _context.Bookings.ToListAsync();
        }

        public Task<List<Booking>> ListByUserAsync(int userId)
        {
            _logger.LogInformation(
                "Repository: Henter booking for bruker {UserId}",
                userId);

            return _context.Bookings
                .Where(b => b.AppUserId == userId)
                .ToListAsync();
        }

        public Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end)
        {
             _logger.LogInformation(
                "Repository: sjekker konflikt for rom {RoomId}",
                roomId);

            // Overlapp-sjekk for å hindre dobbeltbooking av samme rom.
            return _context.Bookings.AnyAsync(b =>
                b.MeetingRoomId == roomId &&
                start < b.EndTime &&
                end > b.StartTime);
        }

        public Task<List<Booking>> GetForRoomOnDayAsync(int roomId, DateTime dayStart, DateTime dayEnd)
        {
            _logger.LogInformation(
                "Repository: Henter booking for rom {RoomId} mellom {DayStart} og {DayEnd}",
                roomId,
                dayStart,
                dayEnd);

            return _context.Bookings
                .Where(b => b.MeetingRoomId == roomId &&
                            b.StartTime < dayEnd &&
                            b.EndTime > dayStart)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            _logger.LogInformation(
                "Repository: Lagrer booking for rom {RoomId}",
                booking.MeetingRoomId);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Repository: Booking {BookingId} lagret suksessfult",
                booking.Id);
        }

        public async Task RemoveAsync(Booking booking)
        {
            _logger.LogInformation(
                "Repository: Fjerner booking {BookingId}",
                booking.Id);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Repository: Booking {BookingId} fjernet suksessfult",
                booking.Id);
        }

        public Task<MeetingRoom?> GetMeetingRoomByIdAsync(int roomId)
        {
            _logger.LogInformation(
                "Repository: Henter møterom {RoomId}",
                roomId);
            return _context.MeetingRooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public Task<AppUser?> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation(
                "Repository:Henter bruker {UserId}",
                userId);
                
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
