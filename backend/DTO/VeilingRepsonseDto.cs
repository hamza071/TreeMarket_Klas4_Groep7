namespace backend.DTO
{
    public class VeilingResponseDto
    {
        public int VeilingID { get; set; }
        public bool Status { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public decimal MinPrijs { get; set; }
        public int TimerInSeconden { get; set; }
        public int ProductID { get; set; }
        public int Hoeveelheid { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EindTimestamp { get; set; } // nieuw veld

       
        public string ProductNaam { get; set; }
        public string Foto { get; set; }
        public string Omschrijving { get; set; }

        
        public string LeverancierNaam { get; set; }
    }
}