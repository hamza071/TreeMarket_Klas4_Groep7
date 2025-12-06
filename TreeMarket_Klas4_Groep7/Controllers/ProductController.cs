using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;

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
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _productService.GetProductenVanVandaag();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product van vandaag kan niet getoond worden.", error = ex.Message });
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
                return StatusCode(500, new { message = "Databasefout: Product van de leverancier kan niet getoond worden.", error = ex.Message });
            }
        }

        // ✅ Haal product op via ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null) return NotFound(new { message = $"Product met ID {id} niet gevonden." });
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon product niet ophalen.", error = ex.Message });
            }
        }

        // ✅ Voeg nieuw product toe of update bestaand product
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken))
                return BadRequest("Artikelkenmerken is verplicht.");

            if (product.Hoeveelheid <= 0)
                return BadRequest("Hoeveelheid moet groter zijn dan 0.");

            if (product.MinimumPrijs < 0)
                return BadRequest("MinimumPrijs mag niet negatief zijn.");

            if (product.Dagdatum.Date < DateTime.Today)
                return BadRequest("Dagdatum mag niet in het verleden liggen.");

            if (product.LeverancierID <= 0)
                return BadRequest("LeverancierID is verplicht.");

            try
            {
                // Roep de service aan om het product toe te voegen of te updaten
                var result = await _productService.AddOrUpdateProduct(product);

                // Als het een nieuw product is, return 201 Created
                if (product.ProductId == 0)
                    return CreatedAtAction(nameof(GetProductById), new { id = result.ProductId }, result);

                // Anders return 200 OK
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product kon niet toegevoegd of geupdate worden.", error = ex.Message });
            }
        }
    }
}