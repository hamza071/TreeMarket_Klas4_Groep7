using backend.DTO;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // GET: api/Product/vandaag
        [HttpGet("vandaag")]
        public async Task<IActionResult> GetVandaag()
        {
            try
            {
                var producten = await _service.GetVandaag();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Product/leverancier
        [HttpGet("leverancier")]
        public async Task<IActionResult> GetMetLeverancier()
        {
            try
            {
                var producten = await _service.GetMetLeverancier();
                return Ok(producten);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _service.GetProductById(id);
                if (product == null)
                    return NotFound(new { message = $"Product {id} niet gevonden." });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/Product/CreateProduct
        [HttpPost("CreateProduct")]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductUploadDto dto)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            if (!User.IsInRole("Leverancier") && !User.IsInRole("Admin"))
                return Forbid();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            try
            {
                var product = await _service.CreateProduct(
                    dto,
                    userId,
                    User.IsInRole("Admin")
                );

                return Ok(new
                {
                    message = "Product succesvol aangemaakt!",
                    product
                });
            }
            // WARNING: expose details only in dev
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    full = ex.ToString()
                });
            }
        }

        // DELETE: api/Product/vandaag
        [HttpDelete("vandaag")]
        [Authorize]
        public async Task<IActionResult> DeleteVandaag()
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            if (!User.IsInRole("Admin") && !User.IsInRole("Leverancier"))
                return Forbid();

            try
            {
                var deleted = await _service.DeleteTodayProducts();
                return Ok(new { deleted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            if (!User.IsInRole("Admin") && !User.IsInRole("Leverancier"))
                return Forbid();

            try
            {
                var success = await _service.DeleteProduct(id);
                if (!success)
                    return NotFound(new { message = $"Product {id} niet gevonden." });

                return Ok(new { message = $"Product {id} verwijderd." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}