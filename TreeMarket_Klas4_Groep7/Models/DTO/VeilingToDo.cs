using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.ToDo
{
    public class VeilingToDo
    {
        

        [Required] // Betekent: mag niet 'null' of 0 zijn
        [Range(1, double.MaxValue, ErrorMessage = "Startprijs moet groter dan 0 zijn.")]
        public decimal StartPrijs { get; set; }

        // gaat dit niet procentueel??
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Prijsstap moet minimaal 1 zijn.")]
        public int PrijsStap { get; set; }

        [Required]
        public DateTime StartTijd { get; set; }

        [Required]
        public DateTime EindTijd { get; set; }
        
        ///////////////////////////////////////////////////////////
        // ff kijken of we deze later nodig hebben voor validatie//
        ///////////////////////////////////////////////////////////
        // [Required]
        // [Range(1, int.MaxValue, ErrorMessage = "Een geldig ProductID is verplicht.")]
        // public int ProductID { get; set; }
        //
        // [Required]
        // [Range(1, int.MaxValue, ErrorMessage = "Een geldig VeilingsmeesterID is verplicht.")]
        // public int VeilingsmeesterID { get; set; }
        
    }
} //