using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TreeMarket_Klas4_Groep7.Views
{
    public class Veilingsmeester: Gebruiker
    {
        [Key]
        public int LeverancierId { get; set; }
        [Required]
        public DateTime PlanDatum { get; set; }

        //Foreign key naar gebruiker
        [ForeignKey(nameof(Gebruiker))]
        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}