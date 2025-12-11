using backend.Interfaces;
using TreeMarket_Klas4_Groep7.Services;
using Microsoft.AspNetCore.Authorization; // <--- BELANGRIJK
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Claim = TreeMarket_Klas4_Groep7.Models.Claim;


namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimController _service;

        public ClaimController(IClaimController service)
        {
            _service = service;
        }

        // GET: api/Claim
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaims()
        {
            try
            {
                // Include Klant en Veiling voor meer info
                var claims = await _service.GetClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        // POST: api/Claim/CreateClaim
        [HttpPost("CreateClaim")]
        [Authorize] // <--- ALLEEN INGELOGDE GEBRUIKERS!
        public async Task<IActionResult> PostClaim([FromBody] ClaimDto claimDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Haal de ingelogde gebruiker ID op uit het token (Identity)
            // Dit is VEEL veiliger dan de ID uit de DTO te halen.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Geeft de GUID string

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Je moet ingelogd zijn om te bieden.");
            }

            try
            {
                var claim = await _service.CreateClaimAsync(claimDto, userId);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Claim mislukt.", error = ex.Message });
            }
        }

        // DELETE: api/Claim/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Alleen admin mag claims verwijderen?
        public async Task<IActionResult> DeleteClaim(int id)
        {
            try 
            {
                var success = await _service.DeleteClaimAsync(id);
                if (!success) return NotFound();

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Fout bij verwijderen.", error = ex.Message });
            }
        }
    }
}