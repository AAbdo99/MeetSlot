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
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public async Task<Booking> CreateBookingAsync(CreateBookingDto dto, int currentUserId)
        {
            _logger.LogInformation(
                "Create booking requested. RoomId={RoomId}, StartTime={StartTime}, EndTime={EndTime}, UserId={UserId}",
                dto.MeetingRoomId,
                dto.StartTime,
                dto.EndTime,
                currentUserId);

            // SIKKERHET: currentUserId kommer fra JWT-claim (controller), aldri fra klientens body.
            // Bekrefter at møterommet finnes før vi bygger booking-objektet.
            var meetingRoom = await _bookingRepository.GetMeetingRoomByIdAsync(dto.MeetingRoomId);
            if (meetingRoom == null)
            {
                _logger.LogWarning(
                    "Create booking rejected. Meeting room not found. RoomId={RoomId}, UserId={UserId}",
                    dto.MeetingRoomId,
                    currentUserId);

                // Meldingsnøkkel: ExceptionMessages.Booking.MoteromIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.MoteromIkkeFunnet);
            }

            // Bekrefter at brukeren finnes før booking opprettes.
            var appUser = await _bookingRepository.GetUserByIdAsync(currentUserId);
            if (appUser == null)
            {
                _logger.LogWarning(
                    "Create booking rejected. User not found. UserId={UserId}",
                    currentUserId);

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
                _logger.LogWarning(
                    "Create booking rejected. Time conflict. RoomId={RoomId}, StartTime={StartTime}, EndTime={EndTime}, UserId={UserId}",
                    dto.MeetingRoomId,
                    dto.StartTime,
                    dto.EndTime,
                    currentUserId);

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
            _logger.LogInformation(
                "Booking created. BookingId={BookingId}, RoomId={RoomId}, UserId={UserId}",
                booking.Id,
                booking.MeetingRoomId,
                booking.AppUserId);

            return booking;
        }

        public async Task DeleteBookingAsync(int id, int currentUserId, bool isAdmin)
        {
            _logger.LogInformation(
                "Delete booking requested. BookingId={BookingId}, CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                id,
                currentUserId,
                isAdmin);

            // SIKKERHET: Ved sletting skiller vi mellom booking som ikke finnes (404) og manglende eierskap (403).
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                _logger.LogWarning(
                    "Delete booking rejected. Booking not found. BookingId={BookingId}, CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                    id,
                    currentUserId,
                    isAdmin);

                // Meldingsnøkkel: ExceptionMessages.Booking.BookingIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.BookingIkkeFunnet);
            }

            if (!isAdmin && booking.AppUserId != currentUserId)
            {
                _logger.LogWarning(
                    "Delete booking rejected. Caller does not own booking. BookingId={BookingId}, OwnerUserId={OwnerUserId}, CallerUserId={CallerUserId}",
                    id,
                    booking.AppUserId,
                    currentUserId);

                throw new ForbiddenException(ExceptionMessages.Autentisering.ManglerTilgang);
            }

            await _bookingRepository.RemoveAsync(booking);
            _logger.LogInformation(
                "Booking deleted. BookingId={BookingId}, CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                id,
                currentUserId,
                isAdmin);
        }

        public Task<List<Booking>> GetBookingsAsync(int currentUserId, bool isAdmin)
        {
            _logger.LogInformation(
                "List bookings requested. CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                currentUserId,
                isAdmin);

            // SIKKERHET: Ikke-admin får kun egne bookinger, filtrert i query mot databasen.
            return isAdmin
                ? _bookingRepository.ListAsync()
                : _bookingRepository.ListByUserAsync(currentUserId);
        }

        public async Task<Booking> GetBookingByIdAsync(int id, int currentUserId, bool isAdmin)
        {
            _logger.LogInformation(
                "Get booking requested. BookingId={BookingId}, CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                id,
                currentUserId,
                isAdmin);

            // SIKKERHET: Ikke-admin kan bare hente booking hvis den eies av currentUserId.
            var booking = isAdmin
                ? await _bookingRepository.GetByIdAsync(id)
                : await _bookingRepository.GetByIdForUserAsync(id, currentUserId);

            if (booking == null)
            {
                _logger.LogWarning(
                    "Get booking rejected. Booking not found or hidden by ownership. BookingId={BookingId}, CallerUserId={CallerUserId}, IsAdmin={IsAdmin}",
                    id,
                    currentUserId,
                    isAdmin);

                // SIKKERHET: Returnerer NotFound også ved manglende eierskap (unngår ID-enumerering).
                // Meldingsnøkkel: ExceptionMessages.Booking.BookingIkkeFunnet
                throw new NotFoundException(ExceptionMessages.Booking.BookingIkkeFunnet);
            }

            return booking;
        }

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(int meetingRoomId, DateTime date)
        {
            _logger.LogInformation(
                "Available slots requested. RoomId={RoomId}, Date={Date}",
                meetingRoomId,
                date.Date);

            var meetingRoom = await _bookingRepository.GetMeetingRoomByIdAsync(meetingRoomId);
            if (meetingRoom == null)
            {
                _logger.LogWarning(
                    "Available slots rejected. Meeting room not found. RoomId={RoomId}, Date={Date}",
                    meetingRoomId,
                    date.Date);

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
