using System.ComponentModel.DataAnnotations;

namespace MeetSlot.Models
{
    public class Booking
    {
        public int Id { get; set; } // primærnøkkel, auto-inkrement

        [Required] // required for å sikre at alle bookinger har et gyldig starttidspunkt, og EndTime må være senere enn StartTime
        public DateTime StartTime { get; set; } // starttidspunkt for bookingen

        [Required]
        public DateTime EndTime { get; set; } // sluttidspunkt for bookingen

        public int MeetingRoomId { get; set; } // fremmednøkkel til MeetingRoom

        [Required]
        public required MeetingRoom MeetingRoom { get; set; } // navigasjonsegenskap for relasjonen til MeetingRoom, forteller at en booking tilhører ett møterom

        public int AppUserId { get; set; } // fremmednøkkel til AppUser

        [Required]
        public required AppUser AppUser { get; set; } // navigasjonsegenskap for relasjonen til AppUser, forteller at en booking tilhører en bruker
    }       
}