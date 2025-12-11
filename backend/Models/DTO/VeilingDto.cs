using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
{
    public class VeilingDto
    {
        [Required]
        public decimal StartPrijs { get; set; }

        [Required]
        public int PrijsStap { get; set; }

        [Required]
        public int ProductID { get; set; }
    }
}