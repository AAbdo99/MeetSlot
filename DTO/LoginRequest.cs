// En DTO (Data Transfer Object) som representerer data sendt fra klienten ved innlogging.
// Kun E-post og passord - ingenting annet skal eksponeres mot API-et.

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(512)]
    public required string Password { get; set; }
}