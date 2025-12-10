using System;
using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class VeilingDto
    {
        [Required] public decimal StartPrijs { get; set; }
        [Required] public int PrijsStap { get; set; }
        [Required] public int ProductID { get; set; }
        [Required] public int VeilingsmeesterID { get; set; }
        [Required] public int TimerInSeconden { get; set; } // nieuw veld
    }
}