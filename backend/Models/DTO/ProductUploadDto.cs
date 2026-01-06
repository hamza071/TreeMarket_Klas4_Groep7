using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
{
    // DTO voor product uploads (via FormData)
    public class ProductUploadDto
    {
        [Required(ErrorMessage = "Productnaam is verplicht.")]
        public string ProductNaam { get; set; } = null!;           // Productnaam

        public string? Varieteit { get; set; }             // Type/variëteit van het product

        [Required(ErrorMessage = "Aantal stuks is verplicht.")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal moet minimaal 1 zijn.")]
        public int Hoeveelheid { get; set; }               // Aantal stuks

        public string? Omschrijving { get; set; }          // Productomschrijving

        [Required(ErrorMessage = "Minimumprijs is verplicht.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimumprijs moet groter dan 0 zijn.")]
        public decimal MinimumPrijs { get; set; }          // Minimumprijs

        public IFormFile? Foto { get; set; }              // Upload afbeelding
    }
}