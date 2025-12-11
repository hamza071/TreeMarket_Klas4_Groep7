namespace backend.Models.DTO
{
    // ✅ DTO = Data Transfer Object
    // Wordt gebruikt om alleen de noodzakelijke Product-data naar de frontend te sturen
    public class ProductDto
    {
        public int ProductId { get; set; }// Uniek ID van het product
        public string Foto { get; set; } // Foto van het product
        public string artikelkenmerken { get; set; }
        public int Hoeveelheid { get; set; }// Beschikbare hoeveelheid
        public decimal MinimumPrijs { get; set; } // Minimumprijs
        public DateTime dagdatum { get; set; } = DateTime.Now;
        public string leverancierID { get; set; }
    }
}