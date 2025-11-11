namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    // ✅ DTO = Data Transfer Object
    // Wordt gebruikt om alleen de noodzakelijke Product-data naar de frontend te sturen
    public class ProductDto
    {
        public int ProductId { get; set; }          // Uniek ID van het product
        public string Foto { get; set; }           // Foto van het product
        public decimal MinimumPrijs { get; set; }  // Minimumprijs
        public int Hoeveelheid { get; set; }       // Beschikbare hoeveelheid
    }
}