using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
{
    public class LeverancierDto
    {
        public int LeverancierId { get; set; }
        [Required(ErrorMessage = "Naam mag niet leeg zijn.")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 tekens bevatten.")]
        public string Naam { get; set; }

        [Required(ErrorMessage = "E-mail mag niet leeg zijn.")]
        [EmailAddress(ErrorMessage = "Een geldig e-mailadres is verplicht.")]
        public string Email { get; set; }

        public string? Telefoonnummer { get; set; }

        [Required(ErrorMessage = "Bedrijf is verplicht.")]
        public string Bedrijf { get; set; }

        [Required(ErrorMessage = "KvK nummer is verplicht.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "KvK nummer moet 8 cijfers bevatten.")]
        public string KvKNummer { get; set; }

        [Required(ErrorMessage = "IBAN is verplicht.")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Ongeldig IBAN nummer.")]
        public string IBANnummer { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minimaal 8 tekens bevatten.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
             ErrorMessage = "Wachtwoord moet minstens één hoofdletter, één cijfer en één speciaal teken bevatten.")]
        public string Wachtwoord { get; set; }
    }
}
