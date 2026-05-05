namespace MeetSlot.Models
{
    public class Booking
    {
        public int Id { get; set; } // Primarnokkel.

        public string Title { get; set; } = string.Empty; // Valgfri tittel for bookingen, kan brukes til å beskrive formålet med møtet.
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; // Tidspunkt for når bookingen ble opprettet, lagres som UTC for konsistens.

        public DateTime StartTime { get; set; } // Starttidspunkt (lagres som UTC).

        public DateTime EndTime { get; set; } // Sluttidspunkt (lagres som UTC).

        public int MeetingRoomId { get; set; } // Fremmednokkel til MeetingRoom.

        public required MeetingRoom MeetingRoom { get; set; } // Dette representerer relasjonen til MeetingRoom, og indikerer at en booking må ha et tilknyttet møterom.

        public int AppUserId { get; set; } // Fremmednokkel til AppUser.

        public required AppUser AppUser { get; set; } // Dette representerer relasjonen til AppUser, og indikerer at en booking må ha en tilknyttet bruker som har opprettet bookingen.
    }       
}