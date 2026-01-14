using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Dashboard
    {
        [Key]
        public int DashboardID { get; set; }
      
        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
}