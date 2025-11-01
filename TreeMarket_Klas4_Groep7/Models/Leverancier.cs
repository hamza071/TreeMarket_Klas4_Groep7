using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TreeMarket_Klas4_Groep7.Views
{
    public class Leverancier : Gebruiker
    {
        [Key]
        public int LeverancierId { get; set; }
        [Required]
        public string bedrijf { get; set; }
        [Required]
        public string KvKNummer { get; set; }
        [Required]
        public string IBANnummer { get; set; }

        //Foreign key naar gebruiker
        [ForeignKey(nameof(Gebruiker))]
        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}