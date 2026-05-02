using System.ComponentModel.DataAnnotations;

namespace MeetSlot.Dtos
{
    public class RegisterRequest // En DTO (Data Transfer Object) som representerer data sendt fra klienten ved registrering av en ny bruker. Den inneholder E-post og Passord, som er nødvendige for å opprette en konto, og har valideringsattributter for å sikre at dataene er i riktig format og oppfyller kravene
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Passordet må være minst 8 tegn")]
        [MaxLength(100)]
        public required string Password { get; set; }

    }
}
