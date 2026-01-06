using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    // 1. De Parent tabel erft van IdentityUser
    public class Gebruiker : IdentityUser
    {
        // VERWIJDERD: Id, Email, Telefoonnummer, Wachtwoord. 
        // Deze zitten al in IdentityUser.

        [Required(ErrorMessage = "Naam mag niet leeg zijn.")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 tekens bevatten.")]
        public string Naam { get; set; }

        // Deze is handig voor EF Core om te weten welk type het is (Klant/Leverancier)
        public string? Discriminator { get; set; }

        // De navigatie properties naar kinderen (Klant, Leverancier) zijn weggehaald.
        // Bij TPT (Table-Per-Type) inheritance regelt EF Core dit zelf.
    }

    // 2. Klant (Child)
    public class Klant : Gebruiker
    {
        // Geen eigen ID nodig, hij gebruikt de 'Id' (string) van IdentityUser
        
        // Je kunt hier specifieke klant-velden toevoegen als je wilt
    }

    // 3. Leverancier (Child)
 
    public class Leverancier : Gebruiker
    {
        // Geen LeverancierId nodig, hij gebruikt de 'Id' van de ouder.

        [Required]
        public string Bedrijf { get; set; } // Let op: Hoofdletter B is standaard in C#

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "KvK nummer moet 8 cijfers bevatten.")]
        public string KvKNummer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Ongeldig IBAN nummer.")]
        public string IBANnummer { get; set; }

        // Relatie: Een leverancier heeft producten
        [JsonIgnore]
        public virtual ICollection<Product> Producten { get; set; } = new List<Product>();
    }

    // 4. Veilingsmeester (Child)

    public class Veilingsmeester : Gebruiker
    {
        [Required]
        public DateTime PlanDatum { get; set; }
    }
}