using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class UpdateStatusDTO
    {
        [Required]
        public bool Status { get; set; }
    }
}