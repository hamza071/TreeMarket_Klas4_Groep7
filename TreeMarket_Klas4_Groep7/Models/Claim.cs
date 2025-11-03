using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Claim
    {
        [Key]
        [Required]
        public int ClaimID { get; set; }

        //Het staat in de RIM als INT, maar de rest is decimal
        [Required]
        public decimal Prijs { get; set; }
        //Foreign key kan gedaan worden in twee methodes:

        //Foreignkeys
        //Kijk! hier wordt 'nameof' toegepast!
        [ForeignKey(nameof(Klant))]
        public int KlantId { get; set; }
        public Klant klant { get; set; }

        //Name of binnen de annotaties zie je dat je gewoon een string kan zetten.
        //NameOf veranderd het naar een string, zonder dat je " " hoeft te gebruiken.
        // nameof kan overal gebruikt worden, ook in Console.WriteLine(), niet alleen in annotaties.
        [ForeignKey(nameof(Veiling))]
        public int VeilingId { get; set; }
        public Veiling Veiling { get; set; }
                      

    }
}