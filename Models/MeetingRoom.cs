using System.ComponentModel.DataAnnotations;

namespace MeetSlot.Models
{
    public class MeetingRoom // representerer et møterom i systemet, med egenskaper som Id, Name, Capacity og Description. Den har også en navigasjonsegenskap Bookings som representerer relasjonen til Booking-modellen, og indikerer at et møterom kan ha mange bookinger.
    {
        public int Id { get; set; } // primærnøkkel, auto-inkrement

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } // unikt navn for møterommet

        [Range(1, 1000)]
        public int Capacity { get; set; } // antall personer som kan være i møterommet

        [MaxLength(500)]
        public string? Description { get; set; } 

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // navigasjonsegenskap for relasjonen til Booking, forteller at et møterom kan ha mange bookinger
    }
}