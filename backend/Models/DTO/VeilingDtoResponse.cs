using System;

namespace backend.Models.DTO
{
    public class VeilingDtoResponse
    {
        public int VeilingID { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public int TimerInSeconden { get; set; }
        public bool Status { get; set; }

        // Koppel product via nested DTO
        public ProductDtoResponse Product { get; set; }

        // Optioneel: Veilingsmeester info
        public VeilingsmeesterDtoResponse Veilingsmeester { get; set; }
    }
}