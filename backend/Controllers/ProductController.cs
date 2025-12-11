using backend.Interfaces;
using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using backend.Data;
using backend.Models;
using backend.Models.DTO;
using backend.Services;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductController _service;

        public ProductController(IProductController service)
        {
            _service = service;
        }

        // ================= GET ENDPOINTS (OPENBAAR) =================

        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _service.GetProductenVanVandaagAsync();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _service.GetProductenMetLeverancierAsync();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // ================= CREATE / UPDATE (BEVEILIGD) =================

        [HttpPost]
        [Authorize(Roles = "Leverancier, Admin")]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] Product product)
        {
            try
            {
                var result = await _service.AddOrUpdateProductAsync(product);
                if (result == null)
                    return BadRequest("Validatie mislukt of product niet gevonden.");
                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // validatiefouten
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // update van niet-bestaand product
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

                var result = await _service.AddOrUpdateProductAsync(product);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
    }
}