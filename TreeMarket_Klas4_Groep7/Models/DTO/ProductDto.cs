using Microsoft.AspNetCore.Http; // Nodig voor IFormFile

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Foto { get; set; } // <-- dit is cruciaal voor file upload
        public string artikelkenmerken { get; set; }
        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }
        public DateTime dagdatum { get; set; } = DateTime.Now;
        public string leverancierID { get; set; }
    }
}