using backend.Interfaces;
using TreeMarket_Klas4_Groep7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Claim = TreeMarket_Klas4_Groep7.Models.Claim;


namespace TreeMarket_Klas4_Groep7.Controllers
{
    // Definieert de route (api/Claim) en geeft aan dat dit een API controller is
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimController _service;
        
        // dependency injection van de service 
        public ClaimController(IClaimController service)
        {
            _service = service;
        }

        // GET: api/Claim
        // Haalt alle claims op
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

        // POST: api/Claim/CreateClaim
        // Maakt een nieuwe claim aan
        [HttpPost("CreateClaim")]
        [Authorize] 
        public async Task<IActionResult> PostClaim([FromBody] ClaimDto claimDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Haalt de userId op uit de token
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

        // DELETE: api/Claim/{id} | Claims verwijderen op basis van ID 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Alleen admin mag claims verwijderen maar dit is geen optie in de frontend 
        public async Task<IActionResult> DeleteClaim(int id)
        {
            try 
            {
                // Probeert      de claim te verwijderen via de service
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