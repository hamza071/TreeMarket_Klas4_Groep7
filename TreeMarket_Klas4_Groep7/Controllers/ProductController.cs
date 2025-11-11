using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Services;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // ✅ Haal producten van vandaag op
        [HttpGet("vandaag")]
        public IActionResult GetVandaag()
        {
            var producten = _productService.GetProductenVanVandaag();
            return Ok(producten);
        }

        // ✅ Haal producten met Leverancier info op
        [HttpGet("leverancier")]
        public IActionResult GetMetLeverancier()
        {
            var producten = _productService.GetProductenMetLeverancier();
            return Ok(producten);
        }

        // ✅ Voeg nieuw product toe of update bestaand product
        [HttpPost]
        public IActionResult CreateOrUpdateProduct(Product product)
        {
            var result = _productService.AddOrUpdateProduct(product);
            return Ok(result);
        }
    }
}




/* using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApiContext _context;

        public ProductController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult CreateProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                //Dit gaat een gebruiker aanmaken
                _context.Product.Add(product);
            }
            else
            {
                var gebruikerInDb = _context.Product.Find(product.ProductId);

                if (gebruikerInDb == null)
                {
                    return new JsonResult(NotFound());
                }

                gebruikerInDb = product;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(product));

        }
    }
}
*/