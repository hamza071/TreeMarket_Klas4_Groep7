using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTO;
using backend.Models;
using Claim = backend.Models.Claim; // Voorkomt verwarring met Security.Claims

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

        // POST: api/Claim/PlaceClaim
        // AANGEPAST: De route heet nu 'PlaceClaim' zodat hij matcht met je React code.
        [HttpPost("PlaceClaim")]
        [Authorize] 
        public async Task<IActionResult> PlaceClaim([FromBody] ClaimDto claimDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Haal de ingelogde gebruiker ID op (String)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Je moet ingelogd zijn om te bieden." });
            }

            try
            {
                // 2. We roepen de service aan. 
                // We verwachten hier true (gelukt) of een exception (mislukt).
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
                // Als de service een fout gooit (bijv: "Niet genoeg voorraad"), sturen we die terug naar React
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