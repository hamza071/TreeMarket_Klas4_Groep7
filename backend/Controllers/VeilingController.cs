using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Group7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingController _service;

        public VeilingController(IVeilingController service)
        {
            _service = service;
        }

        // ================= GET (Openbaar) =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var veilingen = await _service.GetAllAsync();
                return Ok(veilingen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veilingen niet ophalen.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var veiling = await _service.GetByIdAsync(id);
                if (veiling == null)
                    return NotFound(new { message = $"Veiling met ID {id} niet gevonden." });

                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon veiling niet ophalen.", error = ex.Message });
            }
        }

        // ================= CREATE (Alleen Veilingsmeester/Admin) =================
        [HttpPost("CreateVeiling")]
        [Authorize]
        public async Task<IActionResult> CreateVeiling(VeilingDto dto)
        {
            // Haal ID van ingelogde gebruiker uit token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Je hebt geen toestemming om een veiling aan te maken.");

            try
            {
                // Laat de service alles doen: valideer, maak Veiling entity aan, sla op
                var veiling = await _service.CreateVeilingAsync(dto, userId);

                // Stuur volledig object terug naar frontend
                return Ok(new { message = "Veiling aangemaakt", veiling });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Veiling kon niet worden aangemaakt.", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ================= BIEDEN =================
        [HttpPost("Bid")]
        [Authorize]
        public async Task<IActionResult> PlaceBid(CreateBidDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var bid = await _service.PlaceBidAsync(dto, userId);
                if (bid == null) return NotFound("Veiling niet gevonden.");
                return Ok(bid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ================= UPDATE STATUS =================
        [HttpPut("UpdateStatus/{id}")]
        [Authorize(Roles = "Veilingsmeester, Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO dto)
        {
            try
            {
                var veiling = await _service.UpdateStatusAsync(id, dto.Status);
                if (veiling == null) return NotFound("Veiling niet gevonden.");
                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon status niet aanpassen.", error = ex.Message });
            }
        }
    }
}