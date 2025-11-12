using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class KlantDto
    {
        public int GebruikerId { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public string Wachtwoord { get; set; }
    }
}
