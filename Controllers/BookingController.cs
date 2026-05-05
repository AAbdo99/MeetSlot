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

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]             //       POST /api/Booking
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            // SIKKERHET: AppUserId hentes fra token-claim, ikke fra request-body.
            // Dette hindrer at en bruker kan opprette booking på vegne av andre.
            var currentUserId = GetCurrentUserId();
            var booking = await _bookingService.CreateBookingAsync(dto, currentUserId);

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

            // SIKKERHET: Admin kan slette alle bookinger, vanlig bruker kun sine egne.
            await _bookingService.DeleteBookingAsync(id, currentUserId, isAdmin);

            // REST-konsistens: Returner JSON i stedet for ren tekst, slik som resten av API-et.
            return Ok(new { message = "Booking er slettet" });
        }

        [HttpGet]          //       GET /api/Booking
        public async Task<IActionResult> GetBookings()
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            // SIKKERHET: Admin får alle bookinger, vanlig bruker får bare egne.
            var bookings = await _bookingService.GetBookingsAsync(currentUserId, isAdmin); // henter bookinger basert på rolle/eierskap

            return Ok(bookings); // returnerer listen med bookinger
        }

        [HttpGet("{id}")]    //      GET /api/Booking/{id}
        public async Task<IActionResult> GetBookingById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            // SIKKERHET: For ikke-admin returnerer service 404 hvis booking ikke tilhører eier.
            var booking = await _bookingService.GetBookingByIdAsync(id, currentUserId, isAdmin); // henter booking basert på rolle/eierskap

            return Ok(booking); // returnerer booking
        }

        [HttpGet("available-slots")]  //    GET    /api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}`
        public async Task<IActionResult> GetAvailableSlots(int meetingRoomId, DateTime date)
        {
            var availableSlots = await _bookingService.GetAvailableSlotsAsync(meetingRoomId, date);

            return Ok(availableSlots); // Alle ledige slots
        }

        private int GetCurrentUserId()
        {
            // SIKKERHET: NameIdentifier i JWT er eneste kilde for innlogget bruker-id.
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedException(ExceptionMessages.Autentisering.UgyldigeInnloggingsdata);
            }

            return userId;
        }
    }
}
