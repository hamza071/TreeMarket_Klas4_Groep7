using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class AuctionDto
    {
        [Required]
        [MaxLength(100)]
        public string Titel { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Beschrijving { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal StartPrijs { get; set; }

        [Required]
        public DateTime EindDatum { get; set; }
    }
}