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

        public BookingRepository(
            MeetSlotDbContext context,
            ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<Booking?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Repo GetByIdAsync. BookingId={BookingId}", id);
            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<Booking?> GetByIdForUserAsync(int id, int userId)
        {
            _logger.LogDebug("Repo GetByIdForUserAsync. BookingId={BookingId}, UserId={UserId}", id, userId);
            return _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.AppUserId == userId);
        }

        public Task<List<Booking>> ListAsync()
        {
            _logger.LogDebug("Repo ListAsync.");
            return _context.Bookings.ToListAsync();
        }

        public Task<List<Booking>> ListByUserAsync(int userId)
        {
            _logger.LogDebug("Repo ListByUserAsync. UserId={UserId}", userId);
            return _context.Bookings
                .Where(b => b.AppUserId == userId)
                .ToListAsync();
        }

        public Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end)
        {
            _logger.LogDebug(
                "Repo HasConflictAsync. RoomId={RoomId}, StartTime={StartTime}, EndTime={EndTime}",
                roomId,
                start,
                end);

            // Overlapp-sjekk for å hindre dobbeltbooking av samme rom.
            return _context.Bookings.AnyAsync(b =>
                b.MeetingRoomId == roomId &&
                start < b.EndTime &&
                end > b.StartTime);
        }

        public Task<List<Booking>> GetForRoomOnDayAsync(int roomId, DateTime dayStart, DateTime dayEnd)
        {
            _logger.LogDebug(
                "Repo GetForRoomOnDayAsync. RoomId={RoomId}, DayStart={DayStart}, DayEnd={DayEnd}",
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
            try
            {
                _logger.LogDebug(
                    "Repo AddAsync. RoomId={RoomId}, UserId={UserId}",
                    booking.MeetingRoomId,
                    booking.AppUserId);

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while adding booking. RoomId={RoomId}, UserId={UserId}",
                    booking.MeetingRoomId,
                    booking.AppUserId);
                throw;
            }
        }

        public async Task RemoveAsync(Booking booking)
        {
            try
            {
                _logger.LogDebug("Repo RemoveAsync. BookingId={BookingId}", booking.Id);
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while removing booking. BookingId={BookingId}", booking.Id);
                throw;
            }
        }

        public Task<MeetingRoom?> GetMeetingRoomByIdAsync(int roomId)
        {
            _logger.LogDebug("Repo GetMeetingRoomByIdAsync. RoomId={RoomId}", roomId);
            return _context.MeetingRooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public Task<AppUser?> GetUserByIdAsync(int userId)
        {
            _logger.LogDebug("Repo GetUserByIdAsync. UserId={UserId}", userId);
            return _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
