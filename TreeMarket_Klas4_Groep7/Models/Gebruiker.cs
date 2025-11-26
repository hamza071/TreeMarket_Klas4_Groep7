using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Gebruiker
    {
        //=====De annotaties wordt al toegepast in de DTO. Dus dit mag leeg zijn===========
        [Key]
        public int GebruikerId { get; set; }

        //De annotaties zorgen ervoor dat de code ook controleerd of de code daaraan voldoet.
        [Required(ErrorMessage = "Naam mag niet leeg zijn.")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 tekens bevatten.")]
        public string Naam { get; set; }
        [Required(ErrorMessage = "E-mail mag niet leeg zijn.")]
        [EmailAddress(ErrorMessage = "Een geldig e-mailadres is verplicht.")]
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }

        //De wachtwoord en rol wordt gebruikt om de UNIT test uit te voeren.
        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minimaal 8 tekens zijn.")]
        public string Wachtwoord { get; set; }
        [Required]
        public string Rol { get; set; }

        //Binnen de gebruiker klasse de variabele van de child klasses gebruiken.
        //Ik gebruik JsonIgnore annotatie, omdat het anders binnen swagger veel te veel staat.

        //Deze variabeles en annotaties voorkomt de Drop-Database enz. werkt 
        [JsonIgnore]
        [NotMapped]
        public virtual Klant? Klant { get; set; }
        [JsonIgnore]
        [NotMapped]
        public virtual Leverancier? Leverancier { get; set; }
        [JsonIgnore]
        [NotMapped]
        public virtual Veilingsmeester? Veilingsmeester { get; set; }
    }


    //De child (sub) klasses worden hier toegevoegd.
    //Klant, Leverancier en Veilingsmeester

    //Klant child klasse
    public class Klant : Gebruiker
    {

        // alias, EF Core hoeft hier niks van te weten
        [NotMapped]
        //Deze variabele zorgt ervoor dat de klantId overeenkomt met de gebruikersId
        public int KlantId => this.GebruikerId;

        public Klant()
        {
            this.Rol = "Klant";
        }

    }

    //Leverancier child klasse
    public class Leverancier : Gebruiker
    {
        [NotMapped]
        //Deze variabele zorgt ervoor dat de klantId overeenkomt met de gebruikersId
        public int LeverancierId => this.GebruikerId;
        [Required]
        public string bedrijf { get; set; }
        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "KvK nummer moet 8 cijfers bevatten.")]
        public string KvKNummer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Ongeldig IBAN nummer.")] public string IBANnummer { get; set; }

        public ICollection<Product> Producten { get; set; } = new List<Product>();


        public Leverancier() {
            this.Rol = "Leverancier";
        }
    }

    //Veilingsmeester child klasse
    public class Veilingsmeester : Gebruiker
    {
        [NotMapped]
        //Deze variabele zorgt ervoor dat de klantId overeenkomt met de gebruikersId
        public int VeilingsmeesterId => this.GebruikerId;
        [Required]
        public DateTime PlanDatum { get; set; }
        
        public Veilingsmeester()
        {
            this.Rol = "Veilingsmeester";
        }
    }

}