using MeetSlot.Models;

namespace MeetSlot.Repositories.Interfaces
{
    // Kontrakt for dataoperasjoner relatert til booking-flyten.
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);

        // SIKKERHET: Brukes for ikke-admin slik at eierskap filtreres i databasen.
        Task<Booking?> GetByIdForUserAsync(int id, int userId);

        Task<List<Booking>> ListAsync();

        // SIKKERHET: Returnerer kun bookinger eid av userId.
        Task<List<Booking>> ListByUserAsync(int userId);
        Task<bool> HasConflictAsync(int roomId, DateTime start, DateTime end);
        Task<List<Booking>> GetForRoomOnDayAsync(int roomId, DateTime dayStart, DateTime dayEnd);
        Task<MeetingRoom?> GetMeetingRoomByIdAsync(int roomId);
        Task<AppUser?> GetUserByIdAsync(int userId);
        Task AddAsync(Booking booking);
        Task RemoveAsync(Booking booking);
    }
}