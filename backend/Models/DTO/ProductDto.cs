namespace backend.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Foto { get; set; }

        public string Naam { get; set; }           // Productnaam
        public string Varieteit { get; set; }      // Type/variëteit
        public string Omschrijving { get; set; }   // Beschrijving

        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }

        public DateTime Dagdatum { get; set; } = DateTime.Now;

        // Wordt automatisch gevuld door backend
        public string LeverancierID { get; set; }
    }
}