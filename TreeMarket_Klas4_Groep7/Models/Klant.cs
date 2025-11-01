using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TreeMarket_Klas4_Groep7.Views
{
    public class Klant : Gebruiker
    {
        [Key]
        public int KlantID { get; set; }
        //Moet nog iets gedaan worden met een foreign key
        [ForeignKey(nameof(Gebruiker))]
        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}