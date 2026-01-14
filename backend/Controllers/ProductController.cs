using backend.DTO;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

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

    [HttpPost("CreateProduct")]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromForm] ProductUploadDto dto)
    {
        // 1️⃣ Token validatie
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized("Je bent niet ingelogd.");
        }

        // 2️⃣ Rol validatie (DIT is de fix)
        if (!User.IsInRole("Leverancier") && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // 3️⃣ UserId ophalen
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("Je bent niet ingelogd.");
        }

        try
        {
            var product = await _service.PostProduct(dto, userId, User.IsInRole("Admin"));
            return Ok(new
            {
                message = "Product succesvol aangemaakt!",
                product
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Serverfout.", error = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _service.GetProductById(id);
            return product == null ? NotFound(new { message = $"Product {id} niet gevonden." }) : Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/Product/vandaag
    [HttpDelete("vandaag")]
    [Authorize]
    public async Task<IActionResult> DeleteVandaag()
    {
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized("Je bent niet ingelogd.");
        }

        // Authorize roles (Admin or Leverancier)
        if (!User.IsInRole("Admin") && !User.IsInRole("Leverancier"))
        {
            return Forbid();
        }

        try
        {
            var deleted = await _service.DeleteVandaag();
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
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized("Je bent niet ingelogd.");
        }

        // Allow Admin or the supplier (Leverancier) role
        if (!User.IsInRole("Admin") && !User.IsInRole("Leverancier"))
        {
            return Forbid();
        }

        try
        {
            var deleted = await _service.DeleteProduct(id);
            if (!deleted) return NotFound(new { message = $"Product {id} niet gevonden." });
            return Ok(new { message = $"Product {id} verwijderd." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}