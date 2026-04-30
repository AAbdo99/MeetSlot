using System.ComponentModel.DataAnnotations;

namespace MeetSlot.DTO
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public requried string Email {get; set; }

        [Required]
        [MinLengt(8, ErrorMessage ="Passordet må være minst 8 tegn")]
        [MaxLength(100)]
        public required string Password {get; set; }
        
    }
}