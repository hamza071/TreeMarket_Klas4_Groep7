using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class KlantDto
    {
        public int GebruikerId { get; set; }
        [Required(ErrorMessage = "Naam mag niet leeg zijn.")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 tekens bevatten.")]
        public string Naam { get; set; }

        [Required(ErrorMessage = "E-mail mag niet leeg zijn.")]
        [EmailAddress(ErrorMessage = "Een geldig e-mailadres is verplicht.")]
        public string Email { get; set; }

        public string? Telefoonnummer { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minimaal 8 tekens bevatten.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
             ErrorMessage = "Wachtwoord moet minstens één hoofdletter, één cijfer en één speciaal teken bevatten.")]
        public string Wachtwoord { get; set; }

        // DTO die veilig is om naar de frontend te sturen
        public class GebruikerResponseDto
        {
            public int GebruikerId { get; set; }
            public string Naam { get; set; }
            public string Email { get; set; }
            public string Rol { get; set; }
            public string? Telefoonnummer { get; set; }
        }
    }
}
