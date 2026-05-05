using MeetSlot.Dtos;
using MeetSlot.Models;

namespace MeetSlot.Services.Interfaces
{
    // Kontrakt for bookinglogikk mellom controller og repository-lag.
    public interface IBookingService
    {
        // SIKKERHET: currentUserId skal komme fra JWT-claim (NameIdentifier), aldri fra request-body.
        Task<Booking> CreateBookingAsync(CreateBookingDto dto, int currentUserId);

        // SIKKERHET: Admin kan slette alle bookinger, vanlig bruker kun egne.
        Task DeleteBookingAsync(int id, int currentUserId, bool isAdmin);

        // SIKKERHET: Admin får alle bookinger, vanlig bruker får kun egne.
        Task<List<Booking>> GetBookingsAsync(int currentUserId, bool isAdmin);

        // SIKKERHET: Ved manglende eierskap for ikke-admin returneres NotFound (404).
        Task<Booking> GetBookingByIdAsync(int id, int currentUserId, bool isAdmin);
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int meetingRoomId, DateTime date);
    }
}
