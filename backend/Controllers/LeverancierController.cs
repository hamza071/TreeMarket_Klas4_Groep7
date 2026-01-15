using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Nodig voor UserManager
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeverancierController : ControllerBase
    {
        private readonly ILeverancierService _service;

        public LeverancierController(ILeverancierService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllLeveranciers()
        {
            try
            {
                // We halen specifiek de Leveranciers op uit de context
                var leveranciers = await _service.GetAllAsync();
                return Ok(leveranciers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Kon leveranciers niet ophalen.", error = ex.Message });
            }
        }

       
        // GET: Haal leverancier op basis van ID
        // LET OP: ID is nu een STRING (vanwege Identity)
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeverancierById(string id) 
        {
            try
            {
                var leverancier = await _service.GetByIdAsync(id);

                if (leverancier == null)
                    return NotFound(new { message = $"Leverancier niet gevonden met ID: {id}" });

                return Ok(leverancier);
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Leverancier Id kon niet opgehaald worden.", error = ex.Message });
            }
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success) return NotFound(new { message = $"Leverancier niet gevonden met ID: {id}" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateLeverancier([FromBody] Leverancier leverancier)
        {
            
            
            return BadRequest(new { message = "Gebruik het endpoint '/api/Gebruiker/Leverancier' om een account aan te maken." });
        }
    }
}