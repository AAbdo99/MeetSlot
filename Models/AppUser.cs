using System.ComponentModel.DataAnnotations;

namespace MeetSlot.Models
{
    public enum UserRole // enum for å representere brukerroller i systemet, med verdier Admin og User. Dette gjør det enklere å håndtere tilgangskontroll og autorisasjon basert på brukerens rolle
    {
        Admin = 1, // administrator med full tilgang til systemet
        User = 0 // vanlig bruker med begrenset tilgang, kan kun se og administrere egne bookinger
    }

    public class AppUser // representerer en bruker i systemet, med egenskaper som Id, Username, PasswordHash og Role. Den har også en navigasjonsegenskap Bookings som representerer relasjonen til Booking-modellen, og indikerer at en bruker kan ha mange bookinger
    {
        public int Id { get; set; } // primærnøkkel, auto-inkrement
        [Required] // required for å sikre at alle brukere har et gyldig brukernavn, og MaxLength for å begrense lengden på brukernavnet til 100 tegn
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; } // unikt e-post for innlogging

        [Required]
        [MaxLength(512)]
        public required string PasswordHash { get; set; } // hash av passordet for sikker lagring

        public UserRole Role { get; set; } // alle nye brukere får rollen User som standard, og kan oppgraderes til Admin av en administrator

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // navigasjonsegenskap for relasjonen til Booking, forteller at en bruker kan ha mange bookinger
    }
}