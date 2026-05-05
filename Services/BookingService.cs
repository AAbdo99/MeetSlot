using MeetSlot.Dtos;
using MeetSlot.Exceptions;
using MeetSlot.Models;
using MeetSlot.Repositories.Interfaces;
using MeetSlot.Services.Interfaces;

namespace MeetSlot.Services
{
    // Samler bookingregler i ett service-lag før data går til repository.
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Booking> CreateBookingAsync(CreateBookingDto dto, int currentUserId)
        {
            // SIKKERHET: currentUserId kommer fra JWT-claim (controller), aldri fra klientens body.
            // Bekrefter at møterommet finnes før vi bygger booking-objektet.
            var meetingRoom = await _bookingRepository.GetMeetingRoomByIdAsync(dto.MeetingRoomId);
            if (meetingRoom == null)
            {
                // Meldingsnøkkel: ExceptionMessages.Booking.MoteromIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.MoteromIkkeFunnet);
            }

            // Bekrefter at brukeren finnes før booking opprettes.
            var appUser = await _bookingRepository.GetUserByIdAsync(currentUserId);
            if (appUser == null)
            {
                // Meldingsnøkkel: ExceptionMessages.Booking.BrukerIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.BrukerIkkeFunnet);
            }

            // Sjekker om rommet allerede er opptatt i ønsket tidsrom.
            var hasConflict = await _bookingRepository.HasConflictAsync(
                dto.MeetingRoomId,
                dto.StartTime,
                dto.EndTime);

            if (hasConflict)
            {
                // Meldingsnøkkel: ExceptionMessages.Booking.Tidskonflikt
                throw new ConflictException(ExceptionMessages.Booking.Tidskonflikt);
            }

            var booking = new Booking
            {
                Title = dto.Title,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingRoom = meetingRoom,
                AppUser = appUser,
                MeetingRoomId = dto.MeetingRoomId,
                AppUserId = currentUserId
            };

            await _bookingRepository.AddAsync(booking);
            return booking;
        }

        public async Task DeleteBookingAsync(int id, int currentUserId, bool isAdmin)
        {
            // SIKKERHET: Ikke-admin slår bare opp booking med eget userId-filter i databasen.
            var booking = isAdmin
                ? await _bookingRepository.GetByIdAsync(id)
                : await _bookingRepository.GetByIdForUserAsync(id, currentUserId);

            if (booking == null)
            {
                // Meldingsnøkkel: ExceptionMessages.Booking.BookingIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.BookingIkkeFunnet);
            }

            await _bookingRepository.RemoveAsync(booking);
        }

        public Task<List<Booking>> GetBookingsAsync(int currentUserId, bool isAdmin)
        {
            // SIKKERHET: Ikke-admin får kun egne bookinger, filtrert i query mot databasen.
            return isAdmin
                ? _bookingRepository.ListAsync()
                : _bookingRepository.ListByUserAsync(currentUserId);
        }

        public async Task<Booking> GetBookingByIdAsync(int id, int currentUserId, bool isAdmin)
        {
            // SIKKERHET: Ikke-admin kan bare hente booking hvis den eies av currentUserId.
            var booking = isAdmin
                ? await _bookingRepository.GetByIdAsync(id)
                : await _bookingRepository.GetByIdForUserAsync(id, currentUserId);

            if (booking == null)
            {
                // SIKKERHET: Returnerer NotFound også ved manglende eierskap (unngår ID-enumerering).
                // Meldingsnøkkel: ExceptionMessages.Booking.BookingIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.BookingIkkeFunnet);
            }

            return booking;
        }

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int meetingRoomId, DateTime date)
        {
            var meetingRoom = await _bookingRepository.GetMeetingRoomByIdAsync(meetingRoomId);
            if (meetingRoom == null)
            {
                // Meldingsnøkkel: ExceptionMessages.Booking.MoteromIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.MoteromIkkeFunnet);
            }

            // Standard arbeidsdag for slot-visning i UI.
            var dayStart = date.Date.AddHours(8);
            var dayEnd = date.Date.AddHours(16);

            var bookings = await _bookingRepository.GetForRoomOnDayAsync(meetingRoomId, dayStart, dayEnd);
            var availableSlots = new List<AvailableSlotDto>();
            var currentStart = dayStart;

            while (currentStart < dayEnd)
            {
                var currentEnd = currentStart.AddHours(1);

                var hasConflict = bookings.Any(b =>
                    currentStart < b.EndTime &&
                    currentEnd > b.StartTime);

                if (!hasConflict)
                {
                    availableSlots.Add(new AvailableSlotDto
                    {
                        StartTime = currentStart,
                        EndTime = currentEnd
                    });
                }

                currentStart = currentStart.AddHours(1);
            }

            return availableSlots;
        }
    }
}
