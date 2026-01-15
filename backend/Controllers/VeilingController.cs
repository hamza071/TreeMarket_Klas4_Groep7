using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Authorization; // <--- NODIG VOOR BEVEILIGING
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // <--- NODIG OM ID UIT TOKEN TE HALEN

namespace TreeMarket_Klas4_Group7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingService _service;
        private readonly UserManager<Gebruiker> _userManager;

        public VeilingController(
            IVeilingService service,
            UserManager<Gebruiker> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

       
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

        
        [HttpPost("CreateVeiling")]
        [Authorize] // ✔️ alleen check: is ingelogd
        public async Task<IActionResult> CreateVeiling([FromBody] VeilingDto dto)
        {
            //UserId uit token halen
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    message = "Gebruiker is niet correct geauthenticeerd."
                });
            }

            //Gebruiker ophalen
            var gebruiker = await _userManager.FindByIdAsync(userId);
            if (gebruiker == null)
            {
                return Unauthorized(new
                {
                    message = "Gebruiker bestaat niet."
                });
            }

            //Rollen ophalen (JUISTE manier)
            var roles = await _userManager.GetRolesAsync(gebruiker);

            //Autorisatie check
            if (!roles.Contains("Veilingsmeester") && !roles.Contains("Admin"))
            {
                return Forbid("Alleen Veilingsmeester of Admin mag een veiling aanmaken.");
            }

            try
            {
                //Business logic
                var veiling = await _service.CreateVeilingAsync(dto, userId);

                return Ok(new
                {
                    message = "Veiling succesvol aangemaakt.",
                    veiling
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Veiling kon niet worden aangemaakt.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
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

        

        [HttpGet("GetVeilingen")]
        public async Task<ActionResult<List<VeilingResponseDto>>> GetVeilingen()
        {
            var veilingen = await _service.GetAllAsync(); // Zorg dat je service dit return als VeilingResponseDto
            return Ok(veilingen);
        }

        [HttpDelete("DeleteVeiling/{veilingId}")]
        public async Task<IActionResult> DeleteVeiling(int veilingId)
        {
            try
            {
                await _service.DeleteVeilingAsync(veilingId);
                return Ok(new { message = $"Veiling {veilingId} succesvol verwijderd." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Fout bij verwijderen van veiling.", error = ex.Message });
            }
        }
    }
}