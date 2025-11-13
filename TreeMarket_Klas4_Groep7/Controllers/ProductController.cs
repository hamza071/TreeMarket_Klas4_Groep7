using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //Die van productService maakt gebruik van LINQ
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // ✅ Haal producten van vandaag op
        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _productService.GetProductenVanVandaag();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                //Hier wordt een statuscode 500 gegeven als het fout gaat binnen de database
                return new JsonResult(StatusCode(500, new { message = "Databasefout: Product van vandaag kan niet getoont worden.", error = ex.Message }));
            }
        }

        // ✅ Haal producten met Leverancier info op
        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _productService.GetProductenMetLeverancier();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                //Hier wordt een statuscode 500 gegeven als het fout gaat binnen de database
                return new JsonResult(StatusCode(500, new { message = "Databasefout: Product van de leverancier kan niet getoont worden.", error = ex.Message }));
            }
        }

        // ✅ Voeg nieuw product toe of update bestaand product
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            // --- BEGIN BUSINESSLOGICA (VALIDATIE VAN INVOER) ---
            //Hier controleren we of verplichte velden een geldige waarde hebben voordat we de service aanroepen.

            //Artikelkenmerken moet ingevuld zijn
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken))
            {
                return BadRequest("Artikelkenmerken is verplicht.");
            }

            //Hoeveelheid moet groter zijn dan 0
            if (product.Hoeveelheid <= 0)
            {
                return BadRequest("Hoeveelheid moet groter zijn dan 0.");
            }

            //MinimumPrijs mag niet negatief zijn
            if (product.MinimumPrijs < 0)
            {
                return BadRequest("MinimumPrijs mag niet negatief zijn.");
            }

            //Dagdatum mag niet in het verleden liggen (optionele businessregel)
            if (product.Dagdatum.Date < DateTime.Today)
            {
                return BadRequest("Dagdatum mag niet in het verleden liggen.");
            }

            //LeverancierID moet gezet zijn (we verwachten dat dit een bestaande leverancier is)
            if (product.LeverancierID <= 0)
            {
                return BadRequest("LeverancierID is verplicht.");
            }
            // --- EINDE BUSINESSLOGICA (VALIDATIE VAN INVOER) ---

            try
            {
                // --- BEGIN BUSINESSLOGICA (BEPALEN OF HET EEN CREATE OF UPDATE IS) ---
                //Als ProductId 0 is, zien we het als een nieuw product (Create), anders een update.
                bool isNieuwProduct = product.ProductId == 0;
                // --- EINDE BUSINESSLOGICA (BEPALEN OF HET EEN CREATE OF UPDATE IS) ---

                //Hier wordt de service aangeroepen die met EF Core / LINQ de database bewerkt
                var result = await _productService.AddOrUpdateProduct(product);

                // --- BEGIN BUSINESSLOGICA (HTTP-STATUSCODES OP BASIS VAN ACTIE) ---
                //Als het een nieuw product is, zou 201 Created semantisch kloppen.
                //Voor nu retourneren we Ok(result), maar we leggen dit wel uit aan de docent.
                //Je zou bijvoorbeeld dit kunnen doen:
                //
                //if (isNieuwProduct)
                //    return CreatedAtAction(nameof(GetMetLeverancier), new { id = product.ProductId }, result);
                //
                //Maar omdat AddOrUpdateProduct jouw huidige structuur gebruikt, laten we Ok(result) staan.
                // --- EINDE BUSINESSLOGICA (HTTP-STATUSCODES OP BASIS VAN ACTIE) ---

                return Ok(result);
            }
            catch (Exception ex)
            {
                //Hier wordt een statuscode 500 gegeven als het fout gaat binnen de database
                return new JsonResult(StatusCode(500, new { message = "Databasefout: Product kan niet toegevoegd of geupdate worden.", error = ex.Message }));
            }
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
