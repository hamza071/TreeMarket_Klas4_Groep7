using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //Die van productService maakt gebruik van LINQ
        private readonly ProductService _productService;
        private readonly ApiContext _context;

        public ProductController(ProductService productService, ApiContext context)
        {
            _productService = productService;
            _context = context;
        }

        // Haal producten van vandaag op
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
          
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken))
            {
                return BadRequest("Artikelkenmerken is verplicht.");
            }

           
            if (product.Hoeveelheid <= 0)
            {
                return BadRequest("Hoeveelheid moet groter zijn dan 0.");
            }

           
            if (product.MinimumPrijs < 0)
            {
                return BadRequest("MinimumPrijs mag niet negatief zijn.");
            }

            if (product.Dagdatum.Date < DateTime.Today)
            {
                return BadRequest("Dagdatum mag niet in het verleden liggen.");
            }

           
            if (product.LeverancierID <= 0)
            {
                return BadRequest("LeverancierID is verplicht.");
            }
            

            try
            {
                
                bool isNieuwProduct = product.ProductId == 0;
                

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

        //ClaimDTO om gewoon Claim aan te maken
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> PostProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  
            
            try
            {
                var product = new Product
                {
                    Foto = productDto.Foto,
                    Artikelkenmerken = productDto.artikelkenmerken,
                    Hoeveelheid = productDto.Hoeveelheid,
                    MinimumPrijs = productDto.MinimumPrijs,
                    Dagdatum = DateTime.UtcNow,   // server bepaalt datum
                    LeverancierID = productDto.leverancierID
                };

                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();

                return (Ok(product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Product kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? imagePath = null;

            // Afbeelding opslaan
            if (dto.Image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = "/images/" + fileName;
            }

            // Product aanmaken met de juiste properties
            var product = new Product
            {
                Artikelkenmerken = dto.Variety ?? "",  // dit is verplicht in entity
                Hoeveelheid = dto.Quantity,
                MinimumPrijs = dto.MinPrice,
                Foto = imagePath ?? "",                 // dit is verplicht in entity
                Dagdatum = DateTime.UtcNow,
                LeverancierID = dto.LeverancierID
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        //[HttpGet("pending")]
        //public async Task<IActionResult> GetPendingProducts()
        //{
        //    try
        //    {
        //        var pending = await _context.Product
        //            .Where(p => !p.Veilingen.Any()) // nog geen veiling aangemaakt
        //            .Select(p => new
        //            {
        //                code = p.ProductId,
        //                name = p.Artikelkenmerken,
        //                description = "", // kan later uit DTO of extra veld komen
        //                lots = p.Hoeveelheid,
        //                image = p.Foto, // /images/... pad
        //                status = "pending",
        //                productID = p.ProductId,
        //                minPrice = p.MinimumPrijs
        //            })
        //            .ToListAsync();

        //        return Ok(pending);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Kan pending kavels niet ophalen.", error = ex.Message });
        //    }
        //}

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                // Haal het product op inclusief eventuele gerelateerde data (zoals Leverancier)
                var product = await _context.Product
                    .Include(p => p.Leverancier)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                    return NotFound(new { message = "Kavel niet gevonden" });

                // Optioneel: map naar DTO
                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    artikelkenmerken = product.Artikelkenmerken,
                    Hoeveelheid = product.Hoeveelheid,
                    MinimumPrijs = product.MinimumPrijs,
                    Foto = product.Foto,
                    dagdatum = product.Dagdatum,
                    leverancierID = product.LeverancierID
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout bij ophalen kavel", error = ex.Message });
            }
        }

        // Haal alle pending kavels op
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            try
            {
                var pending = await _context.Product
                    .Where(p => !_context.Veiling.Any(v => v.ProductID == p.ProductId))
                    .Select(p => new
                    {
                        code = p.ProductId,
                        name = p.Artikelkenmerken,
                        description = "",
                        lots = p.Hoeveelheid,
                        image = p.Foto,
                        status = "pending",
                        productID = p.ProductId,
                        minPrice = p.MinimumPrijs
                    })
                    .ToListAsync();

                return Ok(pending);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Kan pending kavels niet ophalen.", error = ex.Message });
            }
        }

    }
}