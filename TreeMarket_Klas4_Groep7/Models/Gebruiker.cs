using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TreeMarket_Klas4_Groep7.Models
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
        public string KvKNummer { get; set; }
        [Required]
        public string IBANnummer { get; set; }

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