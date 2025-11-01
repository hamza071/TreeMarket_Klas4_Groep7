using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TreeMarket_Klas4_Groep7.Views
{
    public class Gebruiker
    {
        [Key]
        public int GebruikerId { get; set; }
        [Required]
        public string Naam { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        [Required]
        public string Wachtwoord { get; set; }
        [Required]
        public string Rol { get; set; }

    }
}