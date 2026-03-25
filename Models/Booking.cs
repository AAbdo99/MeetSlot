using System.ComponentModel.DataAnnotations;

namespace MeetSlot.Models
{
    public class Booking
    {
        public int Id { get; set; } // Primarnokkel.

        [Required]
        public DateTime StartTime { get; set; } // Starttidspunkt (lagres som UTC).

        [Required]
        public DateTime EndTime { get; set; } // Sluttidspunkt (lagres som UTC).

        public int MeetingRoomId { get; set; } // Fremmednokkel til MeetingRoom.

        [Required]
        public required MeetingRoom MeetingRoom { get; set; }

        public int AppUserId { get; set; } // Fremmednokkel til AppUser.

        [Required]
        public required AppUser AppUser { get; set; }
    }       
}