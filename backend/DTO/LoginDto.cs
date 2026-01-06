using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-mail mag niet leeg zijn.")]
        [EmailAddress(ErrorMessage = "Een geldig e-mailadres is verplicht.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord mag niet leeg zijn.")]
        public string Wachtwoord { get; set; }
    }
}
