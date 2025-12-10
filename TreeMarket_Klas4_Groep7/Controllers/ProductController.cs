using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly ProductService _productService;
        private readonly ApiContext _context;

        public ProductController(ProductService productService, ApiContext context)
        {
            _productService = productService;
            _context = context;
        }

        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                // Gebruik de service-methode die ook Leverancier-info teruggeeft
                var producten = await _productService.GetProductenMetLeverancier();

                var result = producten.Select(p => new ProductMetVeilingmeesterDto
                {
                    ProductId = p.ProductId,
                    MinimumPrijs = p.MinimumPrijs,
                    // Gebruik de leverancier-naam property die door de service DTO levert.
                    // Vervang 'LeverancierNaam' door de correcte propertynaam als de DTO anders heet (bv. 'UserName' of 'Naam').
                    LeverancierNaam = (p as dynamic).LeverancierNaam ?? (p as dynamic).UserName ?? "Onbekend"
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // ================= CREATE / UPDATE (BEVEILIGD) =================

        [HttpPost]
        [Authorize(Roles = "Leverancier, Admin")] // Alleen Leveranciers mogen dit!
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            // 1. Validatie
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken)) return BadRequest("Artikelkenmerken is verplicht.");
            if (product.Hoeveelheid <= 0) return BadRequest("Hoeveelheid moet groter zijn dan 0.");
            if (product.MinimumPrijs < 0) return BadRequest("MinimumPrijs mag niet negatief zijn.");
            if (product.Dagdatum.Date < DateTime.Today) return BadRequest("Dagdatum mag niet in het verleden liggen.");

            // AANGEPAST: String check voor ID
            if (string.IsNullOrEmpty(product.LeverancierID)) return BadRequest("LeverancierID is verplicht.");

            try
            {
                // Omdat je service waarschijnlijk nog niet is geüpdatet voor strings, 
                // doen we het hier even direct (of je past je service aan):
                
                if (product.ProductId == 0) // Nieuw
                {
                    await _context.Product.AddAsync(product);
                }
                else // Update
                {
                    _context.Product.Update(product);
                }
                
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // DIT IS DE BETERE METHODE (gebruikt DTO en Token)
        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Leverancier")] // Alleen Leveranciers
        public async Task<IActionResult> PostProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Haal de ID van de ingelogde gebruiker op (uit token)
            // Dit is VEILIGER dan de ID uit de DTO halen!
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            
            // (Als Admin dit doet, moeten we misschien wel de ID uit de DTO pakken, 
            // maar voor een Leverancier is dit het veiligst).
            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

            try
            {
                var product = new Product
                {
                    Foto = productDto.Foto,
                    Artikelkenmerken = productDto.artikelkenmerken,
                    Hoeveelheid = productDto.Hoeveelheid,
                    MinimumPrijs = productDto.MinimumPrijs,
                    Dagdatum = DateTime.UtcNow,
                    
                    // AANGEPAST: Gebruik de ID uit het token!
                    LeverancierID = userId 
                };

                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        [HttpPost("Upload")]
        [Authorize(Roles = "Leverancier")]
        public async Task<IActionResult> Upload([FromForm] ProductUploadDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Geen geldige login.");

            // === FOTO OPSLAAN ===
            string fotoPad = null;

            if (dto.Image != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                fotoPad = "/images/" + fileName;
            }

            // === PRODUCT MAKEN ===
            var product = new Product
            {
                Foto = fotoPad,
                Artikelkenmerken = dto.Title + (dto.Variety != null ? " - " + dto.Variety : ""),
                Hoeveelheid = dto.Quantity,
                MinimumPrijs = dto.MinPrice,
                Dagdatum = DateTime.UtcNow,
                LeverancierID = userId
            };

            await _context.Product.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }
    }
}