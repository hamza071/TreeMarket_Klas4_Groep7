using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMarket_Klas4_Groep7.Views
{
    public class Dashboard
    {
        [Key]
        public int DashboardID { get; set; }
        //Foreign key
        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
}