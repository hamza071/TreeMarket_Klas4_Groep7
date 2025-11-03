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
        [JsonIgnore]
        public virtual Klant? Klant { get; set; }
        [JsonIgnore]
        public virtual Leverancier? Leverancier { get; set; }
        [JsonIgnore]
        public virtual Veilingsmeester? Veilingsmeester { get; set; }
    }


    //De child (sub) klasses worden hier toegevoegd.
    //Klant, Leverancier en Veilingsmeester

    //Klant child klasse
    public class Klant : Gebruiker
    {
        public Klant()
        {
            this.Rol = "Klant";
        }

    }

    //Leverancier child klasse
    public class Leverancier : Gebruiker
    {
        [Required]
        public string bedrijf { get; set; }
        [Required]
        public string KvKNummer { get; set; }
        [Required]
        public string IBANnummer { get; set; }

        public Leverancier() {
            this.Rol = "Leverancier";
        }
    }

    //Veilingsmeester child klasse
    public class Veilingsmeester : Gebruiker
    {
        [Required]
        public DateTime PlanDatum { get; set; }
        
        public Veilingsmeester()
        {
            this.Rol = "Veilingsmeester";
        }
    }

}