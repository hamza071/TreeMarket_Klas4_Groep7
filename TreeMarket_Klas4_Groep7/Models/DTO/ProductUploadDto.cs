using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    // DTO voor product uploads (via FormData)
    public class ProductUploadDto
    {
        [Required(ErrorMessage = "Titel is verplicht.")]
        public string Title { get; set; } = null!; // verplicht

        public string? Variety { get; set; } // optioneel

        [Required(ErrorMessage = "Aantal stuks is verplicht.")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal moet minimaal 1 zijn.")]
        public int Quantity { get; set; }

        public string? Description { get; set; } // optioneel

        [Required(ErrorMessage = "Minimumprijs is verplicht.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimumprijs moet groter dan 0 zijn.")]
        public decimal MinPrice { get; set; }

        // Image upload via FormData, optioneel
        public IFormFile? Image { get; set; }
    }
}