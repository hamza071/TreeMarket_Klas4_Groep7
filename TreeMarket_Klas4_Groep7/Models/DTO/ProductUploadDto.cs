using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class ProductUploadDto
    {
        [Required]
        public string Title { get; set; }

        public string? Variety { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MinPrice { get; set; }

        // Image upload via FormData
        public IFormFile? Image { get; set; }
    }
}