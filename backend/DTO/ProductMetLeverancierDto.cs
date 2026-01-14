namespace backend.DTO
{
    public class ProductMetLeverancierDto
    {
       
        public int ProductId { get; set; }
        public string LeverancierNaam { get; set; }

        public string Naam { get; set; }
        public string Variëteit { get; set; }
        public string KleurOfSoort { get; set; }
        public int AantalStuks { get; set; }
        public string Omschrijving { get; set; }
        public decimal MinimumPrijs { get; set; }
        public string AfbeeldingUrl { get; set; }
        
        public int LeverancierId { get; set; } 

        public decimal StartPrijs { get; set; }
        public int PrijsStap { get; set; }
    }
}