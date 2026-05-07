using MeetSlot.Dtos;
using MeetSlot.Exceptions;
using MeetSlot.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeetSlot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Alle endepunkter krever innlogging
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController>logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]             //       POST /api/Booking
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            // SIKKERHET: AppUserId hentes fra token-claim, ikke fra request-body.
            // Dette hindrer at en bruker kan opprette booking på vegne av andre.
            var currentUserId = GetCurrentUserId();

             _logger.LogInformation(
                "Bruker {UserId} Oppretter booking av rom {MeetingRoomId} fra {StartTime} til {EndTime}",
                currentUserId,
                dto.MeetingRoomId,
                dto.StartTime,
                dto.EndTime);

            var booking = await _bookingService.CreateBookingAsync(dto, currentUserId);

             _logger.LogInformation(
                "Booking {BookingId} opprettet suksessfult av bruker {UserId}",
                booking.Id,
                currentUserId);

            return Ok(new
            {
                booking.Id,
                booking.Title,
                booking.StartTime,
                booking.EndTime,
                booking.MeetingRoomId,
                booking.AppUserId
            });
        }

        [HttpDelete("{id}")]          //       DELETE /api/Booking/{id}
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Bruker {UserId} sletter booking {BookingId}. IsAdmin: {IsAdmin}",
                currentUserId,
                id,
                isAdmin);

            // SIKKERHET: Admin kan slette alle bookinger, vanlig bruker kun sine egne.
            await _bookingService.DeleteBookingAsync(id, currentUserId, isAdmin);

            _logger.LogInformation(
                "Booking {BookingId} slettet suksessfult av bruker {UserId}",
                id,
                currentUserId);

            // REST-konsistens: Returner JSON i stedet for ren tekst, slik som resten av API-et.
            return Ok(new { message = "Booking er slettet" });
        }

        [HttpGet]          //       GET /api/Booking
        public async Task<IActionResult> GetBookings()
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Bruker {UserId} ber om booking. IsAdmin: {IsAdmin}",
                currentUserId,
                isAdmin);

            // SIKKERHET: Admin får alle bookinger, vanlig bruker får bare egne.
            var bookings = await _bookingService.GetBookingsAsync(currentUserId, isAdmin); // henter bookinger basert på rolle/eierskap

            _logger.LogInformation(
                "Booking returnert for bruker {UserId}. IsAdmin: {IsAdmin}",
                currentUserId,
                isAdmin);

            return Ok(bookings); // returnerer listen med bookinger
        }

        [HttpGet("{id}")]    //      GET /api/Booking/{id}
        public async Task<IActionResult> GetBookingById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "bruker {UserId} ber etter booking {BookingId}. IsAdmin: {IsAdmin}",
                currentUserId,
                id,
                isAdmin);

            // SIKKERHET: For ikke-admin returnerer service 404 hvis booking ikke tilhører eier.
            var booking = await _bookingService.GetBookingByIdAsync(id, currentUserId, isAdmin); // henter booking basert på rolle/eierskap

            _logger.LogInformation(
                "Booking {BookingId} returnert til bruker {UserId}",
                id,
                currentUserId);

            return Ok(booking); // returnerer booking
        }

        [HttpGet("available-slots")]  //    GET    /api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}`
        public async Task<IActionResult> GetAvailableSlots(int meetingRoomId, DateTime date)
        {
            
            _logger.LogInformation(
                "Ledig plass etterspurt for rom {MeetingRoomId} for den {Date}",
                meetingRoomId,
                date.Date);

            var availableSlots = await _bookingService.GetAvailableSlotsAsync(meetingRoomId, date);

            _logger.LogInformation(
                "Ledige plasser returnert for rom {MeetingRoomId} for den {Date}",
                meetingRoomId,
                date.Date);

            return Ok(availableSlots); // Alle ledige slots
        }

        private int GetCurrentUserId()
        {
            // SIKKERHET: NameIdentifier i JWT er eneste kilde for innlogget bruker-id.
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                 _logger.LogWarning("ugjyldig eller manglende user id in JWT token");
                throw new UnauthorizedException(ExceptionMessages.Autentisering.UgyldigeInnloggingsdata);
            }

            return userId;
        }
    }
}
