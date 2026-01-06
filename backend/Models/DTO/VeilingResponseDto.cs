namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class VeilingResponseDto
    {
        public int VeilingID { get; set; }
        public bool Status { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public int TimerInSeconden { get; set; }
        public int ProductID { get; set; }

        // Product details
        public string ProductNaam { get; set; }
        public string Foto { get; set; }
    }
}