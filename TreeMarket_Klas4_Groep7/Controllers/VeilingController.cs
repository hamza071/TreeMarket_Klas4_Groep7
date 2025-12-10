using backend.Interfaces;
using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // <--- NODIG OM ID UIT TOKEN TE HALEN
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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

        // GET: api/veiling
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

        // GET: api/veiling/5
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
        [Authorize(Roles = "Veilingsmeester, Admin")] // <--- BEVEILIGING
        public async Task<IActionResult> Create(VeilingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Haal de ID van de ingelogde gebruiker op uit het token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized("Je bent niet ingelogd.");

            try
            {
                var veiling = new Veiling
                {
                    StartPrijs = dto.StartPrijs,
                    HuidigePrijs = dto.StartPrijs,
                    PrijsStap = dto.PrijsStap,
                    // PrijsStrategie = dto.PrijsStrategie, // Voeg toe als je die in DTO hebt
                    ProductID = dto.ProductID,

                    // AANGEPAST: We gebruiken de ID uit het token, niet uit de DTO!
                    VeilingsmeesterID = userId,

                    Status = true,
                    // TimerInSeconden = dto.TimerInSeconden // Voeg toe als je die in DTO hebt
                };

                await _service.CreateVeilingAsync(dto, userId);

                return Ok(veiling);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veiling kon niet worden aangemaakt.", error = ex.Message });
            }
        }

        // ================= BIEDEN (Alleen Ingelogde Klanten) =================

        // POST: api/veiling/Bid
        [HttpPost("Bid")]
        [Authorize] // <--- Iedereen met een account mag bieden
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

        // ================= UPDATE (Alleen Veilingsmeester/Admin) =================

        // PUT: api/veiling/UpdateStatus/5
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