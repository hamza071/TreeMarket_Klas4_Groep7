using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Views;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    public class HomeController : Controller
    {
        //Deze functie toont de index.cshtml
        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product{ Id=1, Name="Apple", Price=0.50M },
                new Product{ Id=2, Name="Banana", Price=0.25M }
            };
            // Use a physical view path if the file is not in Views/Home/

            return View("~/Front-End/home/index.cshtml");
        }

        //Voor andere functies die we gebruiken om de frontend te tonen
    }
}