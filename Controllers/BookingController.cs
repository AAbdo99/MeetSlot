using MeetSlot.Data;
using MeetSlot.Dtos;
using MeetSlot.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetSlot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly MeetSlotDbContext _context;

        public BookingController(MeetSlotDbContext context)
        {
            _context = context;
        }

        [HttpPost]             //       POST /api/Booking
        public IActionResult CreateBooking(CreateBookingDto dto)
        {
            var meetingRoom = _context.MeetingRooms.Find(dto.MeetingRoomId);// returnerer feil hvis MeetingRoomId ikke finnes i databasen
            if (meetingRoom == null)   
            {
                return BadRequest("Møterommet ble ikke funnet");  // Error:  400
            }

            var appUser = _context.AppUsers.Find(dto.AppUserId);// returnerer feil hvis brukeren ikke finnes i databasen
            if (appUser == null)      
            {
                return BadRequest("Brukeren ble ikke funnet");   // Error:  400
            }

            if (dto.EndTime <= dto.StartTime)  // stopper ugyldig bookingtid
            {
                return BadRequest("Slutttidspunkt må være etter starttidspunkt");  // Error:  400
            }

            var hasConflict = _context.Bookings.Any(b =>   
                b.MeetingRoomId == dto.MeetingRoomId &&
                dto.StartTime < b.EndTime &&
                dto.EndTime > b.StartTime);  // sjekker om rommet allerede er booket i samme tidsrom i DataBase

            if (hasConflict)  
            {
                return BadRequest("Dette rommet er allerede booket for det valgte tidspunktet.");  // Error:  400 Not Found 
            }

            var booking = new Booking
            {
                Title = dto.Title,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingRoom = meetingRoom,
                AppUser = appUser,
                MeetingRoomId = dto.MeetingRoomId,
                AppUserId = dto.AppUserId
            };  // oppretter et Booking-objekt i minnet basert på data fra DTO før lagring i databasen

            _context.Bookings.Add(booking); // legger til booking
            _context.SaveChanges();   // lagrer endringer

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
        public IActionResult DeleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id); // finner booking med gitt id fra Database

            if (booking == null) 
            {
                return NotFound("Booking ble ikke funnet.");  // Error:  404 Not Found 
            }

            _context.Bookings.Remove(booking);// sletter booking
            _context.SaveChanges();   // oppdaterer databasen

            return Ok("Booking er slettet");   // OK 200
        }



        [HttpGet]          //       GET /api/Booking
        public IActionResult GetBookings()
        {
            var bookings = _context.Bookings.ToList(); // henter alle bookinger

            return Ok(bookings); // returnerer listen med bookinger
        }



        [HttpGet("{id}")]    //      GET /api/Booking/{id}
        public IActionResult GetBookingById(int id)
        {
            var booking = _context.Bookings.Find(id); // henter booking med gitt id

            if (booking == null) 
            {
                return NotFound("Booking ble ikke funnet."); // returnerer 404 hvis booking ikke finnes
            }

            return Ok(booking); // returnerer booking
        }



        [HttpGet("available-slots")]  //    GET    /api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}`
        public IActionResult GetAvailableSlots(int meetingRoomId, DateTime date)
        {
            var meetingRoom = _context.MeetingRooms.Find(meetingRoomId); // sjekker at rommet finnes

            if (meetingRoom == null)
            {
                return NotFound("Møterom ble ikke funnet.");//  Error: 404 
            }

            var dayStart = date.Date.AddHours(8); // start på arbeidsdagen
            var dayEnd = date.Date.AddHours(16); // slutt på arbeidsdagen

            var bookings = _context.Bookings
                .Where(b => b.MeetingRoomId == meetingRoomId &&
                            b.StartTime < dayEnd &&
                            b.EndTime > dayStart)
                .ToList(); // henter bookinger for valgt rom og dato

            var availableSlots = new List<AvailableSlotDto>();

            var currentStart = dayStart;

            while (currentStart < dayEnd)
            {
                var currentEnd = currentStart.AddHours(1); // lager 1-times slot

                var hasConflict = bookings.Any(b =>
                    currentStart < b.EndTime &&
                    currentEnd > b.StartTime); // sjekker om sloten kolliderer med en booking

                if (!hasConflict)
                {
                    availableSlots.Add(new AvailableSlotDto // legger til ledig slot
                    {
                        StartTime = currentStart,
                        EndTime = currentEnd
                    });
                }

                currentStart = currentStart.AddHours(1); // går videre til neste slot
            }

            return Ok(availableSlots); // Alle ledige slots
        }




    }
}