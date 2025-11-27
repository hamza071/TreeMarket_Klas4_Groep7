using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.ToDo
{
    public class VeilingToDo
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Startprijs moet groter dan 0 zijn.")]
        public decimal StartPrijs { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Prijsstap moet minimaal 1 zijn.")]
        public int PrijsStap { get; set; }

        // Sluitingstijd in seconden (vervangt StartTijd en EindTijd)
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Sluitingstijd moet minimaal 1 seconde zijn.")]
        public int SluitingstijdInSeconden { get; set; }

        ///////////////////////////////////////////////////////////
        // Optioneel: ProductID of VeilingsmeesterID als je validatie wilt
        ///////////////////////////////////////////////////////////
        // [Required]
        // [Range(1, int.MaxValue, ErrorMessage = "Een geldig ProductID is verplicht.")]
        // public int ProductID { get; set; }
        //
        // [Required]
        // [Range(1, int.MaxValue, ErrorMessage = "Een geldig VeilingsmeesterID is verplicht.")]
        // public int VeilingsmeesterID { get; set; }
    }
}