using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMarket_Klas4_Groep7.Models;

[Table("Veilingsmeester")]
public class Veilingsmeester : Gebruiker
{
    // PlanDatum is extra info specifiek voor Veilingsmeester
    [Required]
    public DateTime PlanDatum { get; set; }

    // Extra properties zoals Naam, Telefoon, etc. komen via Gebruiker / IdentityUser
}