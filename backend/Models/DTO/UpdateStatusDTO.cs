using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
{
    public class UpdateStatusDTO
    {
        [Required]
        public bool Status { get; set; }
    }
}