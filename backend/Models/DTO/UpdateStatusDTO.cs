using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class UpdateStatusDTO
    {
        [Required]
        public bool Status { get; set; }
    }
}