namespace MeetSlot.Models
{
    // Roller brukt for autorisasjon i systemet.
    public enum UserRole
    {
        Admin = 1, // administrator med full tilgang til systemet
        User = 0 // vanlig bruker med begrenset tilgang, kan kun se og administrere egne bookinger
    }

    public class AppUser
    {
        public int Id { get; set; } // Primarnokkel.
        public required string Email { get; set; }

        public required string PasswordHash { get; set; } // Hash av passord.

        public UserRole Role { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // Navigasjonsegenskap for relasjonen til Booking, forteller at en bruker kan ha mange bookinger.
    }
}