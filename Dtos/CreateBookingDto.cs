using System.ComponentModel.DataAnnotations;
using MeetSlot.Validation;

namespace MeetSlot.Dtos
{
    public class CreateBookingDto  // brukes når en ny booking opprettes
    {
        // SIKKERHET: AppUserId er med vilje IKKE et felt i DTO.
        // Bruker-id skal alltid komme fra JWT-claim i controlleren,
        // slik at klienten ikke kan booke i en annen brukers navn.

        // VALIDERING: Title må være minst 1 tegn (tom streng tillates ikke).
        // [Required] alene tillater tom streng "" fordi det ikke er null.
        // AllowEmptyStrings=false og StringLength with MinimumLength=1 blokkerer tom streng eksplisitt.
        [Required(AllowEmptyStrings = false)]
        [StringLength(120, MinimumLength = 1)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [DateGreaterThan(nameof(StartTime), ErrorMessage = "Slutttidspunkt må være etter starttidspunkt.")]
        public DateTime EndTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MeetingRoomId må være større enn 0.")]
        public int MeetingRoomId { get; set; }
    }
}

