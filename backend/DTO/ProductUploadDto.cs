using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class ProductUploadDto
    {
        [Required(ErrorMessage = "Productnaam is verplicht.")]
        public string ProductNaam { get; set; } = null!;           

        public string? Varieteit { get; set; }             

        [Required(ErrorMessage = "Aantal stuks is verplicht.")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal moet minimaal 1 zijn.")]
        public int Hoeveelheid { get; set; }             

        public string? Omschrijving { get; set; }         

        [Required(ErrorMessage = "Minimumprijs is verplicht.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimumprijs moet groter dan 0 zijn.")]
        public decimal MinimumPrijs { get; set; }          

        public IFormFile? Foto { get; set; }              // Upload van de afbeelding
    }
}