using Microsoft.AspNetCore.Identity;

namespace TreeMarket_Klas4_Groep7.Views
{
    public class Gebruiker
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public string Wachtwoord { get; set; }
        public string Rol { get; set; }

    }
}
