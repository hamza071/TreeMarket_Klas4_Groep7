using backend.Interfaces;
using backend.Models;
using backend.Models.DTO;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Models.DTO;

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
        => Ok(await _service.GetProductenVanVandaagAsync());

    [HttpGet("leverancier")]
    public async Task<IActionResult> GetMetLeverancier()
        => Ok(await _service.GetProductenMetLeverancierAsync());

    [HttpPost("CreateProduct")]
    [Authorize(Roles = "Leverancier,Admin")]
    public async Task<IActionResult> CreateProduct([FromForm] ProductUploadDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var product = await _service.CreateProductAsync(
            dto,
            userId,
            User.IsInRole("Admin"));

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Leverancier,Admin")]
    public async Task<IActionResult> CreateOrUpdate([FromBody] Product product)
        => Ok(await _service.AddOrUpdateProductAsync(product));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}