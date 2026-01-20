using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTO;
using backend.Models;
using Claim = backend.Models.Claim; 

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService _service;

        public ClaimController(IClaimService service)
        {
            _service = service;
        }

        // GET: api/Claim
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaims()
        {
            try
            {
                var claims = await _service.GetClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
        
        //====De history stukje om de gekochte claimgeschiedenis te zien=========
        // GET: api/Claim/GetHistory
        [HttpGet("GetHistory")]
        public async Task<IActionResult> GetHistory(string productNaam, string leverancierNaam)
        {
            // Als er geen productnaam is, kunnen we niks zoeken
            if (string.IsNullOrEmpty(productNaam))
                return BadRequest("Productnaam is verplicht.");

            try
            {
                // Roep de SQL Service aan
                var history = await _service.GetHistoryAsync(productNaam, leverancierNaam);
                return Ok(history);
            }
            catch (Exception ex)
            {
                // Log de fout en geef een nette melding terug
                Console.WriteLine($"Fout bij historie ophalen: {ex.Message}");
                return StatusCode(500, new { message = "Kon historie niet ophalen.", error = ex.Message });
            }
        }
        // ========================================

        // POST: api/Claim/PlaceClaim
        [HttpPost("PlaceClaim")]
        [Authorize] 
        public async Task<IActionResult> PlaceClaim([FromBody] ClaimDto claimDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Je moet ingelogd zijn om te bieden." });
            }

            try
            {
                var result = await _service.VerwerkAankoopAsync(claimDto, userId);

                if (result)
                {
                    return Ok(new { message = "Gefeliciteerd! Aankoop succesvol verwerkt." });
                }
                else
                {
                    return BadRequest(new { message = "Kon aankoop niet verwerken (mogelijk onvoldoende voorraad)." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Claim/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            try 
            {
                var success = await _service.DeleteClaimAsync(id);
                if (!success) return NotFound(new { message = "Claim niet gevonden." });

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Fout bij verwijderen.", error = ex.Message });
            }
        }
    }
}