using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class LeverancierDto
    {
        public int LeverancierId { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public string Bedrijf { get; set; }
        public string KvKNummer { get; set; }
        public string IBANnummer { get; set; }
        public string Wachtwoord { get; set; }
    }
}
