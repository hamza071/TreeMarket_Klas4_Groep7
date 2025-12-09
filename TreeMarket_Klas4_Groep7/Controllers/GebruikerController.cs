using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private readonly UserManager<Gebruiker> _userManager;
        private readonly ApiContext _context;

        // We injecteren UserManager (voor logica) en ApiContext (voor lijstjes ophalen)
        public GebruikerController(UserManager<Gebruiker> userManager, ApiContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ================= REGISTRATIE ENDPOINTS =================

        [HttpPost("Klant")]
        public async Task<IActionResult> CreateUserKlant([FromBody] KlantDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Identity vereist een UserName. We gebruiken hiervoor het Email adres.
            var klant = new Klant
            {
                UserName = dto.Email, 
                Email = dto.Email,
                Naam = dto.Naam,
                PhoneNumber = dto.Telefoonnummer,
                EmailConfirmed = true // Zetten we op true zodat ze direct kunnen inloggen
            };

            // 1. Maak gebruiker aan (Wachtwoord wordt hier automatisch gehasht!)
            var result = await _userManager.CreateAsync(klant, dto.Wachtwoord);

            if (result.Succeeded)
            {
                // 2. Rol toewijzen
                await _userManager.AddToRoleAsync(klant, "Klant");
                return Ok(new { message = "Klant succesvol geregistreerd!" });
            }
            
            // Als het mislukt (bijv. wachtwoord te zwak), stuur fouten terug
            return BadRequest(result.Errors);
        }

        [HttpPost("Leverancier")]
        public async Task<IActionResult> CreateUserLeverancier([FromBody] LeverancierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var leverancier = new Leverancier
            {
                UserName = dto.Email,
                Email = dto.Email,
                Naam = dto.Naam,
                PhoneNumber = dto.Telefoonnummer,
                Bedrijf = dto.Bedrijf,
                KvKNummer = dto.KvKNummer,
                IBANnummer = dto.IBANnummer,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(leverancier, dto.Wachtwoord);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(leverancier, "Leverancier");
                return Ok(new { message = "Leverancier succesvol geregistreerd!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("Veilingsmeester")]
        public async Task<IActionResult> CreateUserVeilingsmeester([FromBody] VeilingsmeesterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vm = new Veilingsmeester
            {
                UserName = dto.Email,
                Email = dto.Email,
                Naam = dto.Naam,
                PhoneNumber = dto.Telefoonnummer,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(vm, dto.Wachtwoord);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(vm, "Veilingsmeester");
                return Ok(new { message = "Veilingsmeester succesvol geregistreerd!" });
            }

            return BadRequest(result.Errors);
        }

        // ================= BEHEER FUNCTIES (Alleen voor Admin) =================

        // GET: api/Gebruiker/GetAllUsers
        [Authorize(Roles = "Admin")] // Beveiligd met Identity Roles
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            // We gebruiken _context.Users omdat UserManager geen simpele "ToList" heeft voor alle types
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // DELETE: api/Gebruiker/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id) // Let op: ID is nu een string!
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Gebruiker niet gevonden.");

            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }

  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id) // Let op: ID is nu een string!
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Gebruiker niet gevonden.");
            
            return Ok(user);
        }

        // ================= BEVEILIGDE ROUTE: Huidige ingelogde gebruiker =================
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Haal GebruikerId uit de JWT claims
                var userIdClaim = User.FindFirst("GebruikerId")?.Value;
                if (userIdClaim == null) return Unauthorized("Gebruiker niet gevonden in token.");

                int userId = int.Parse(userIdClaim);

                var gebruiker = await _context.Gebruiker
                    .Where(g => g.GebruikerId == userId)
                    .Select(g => new
                    {
                        g.GebruikerId,
                        g.Naam,
                        g.Email,
                        g.Rol,
                        g.Telefoonnummer
                    })
                    .FirstOrDefaultAsync();

                if (gebruiker == null) return NotFound("Gebruiker niet gevonden.");

                return Ok(gebruiker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout bij ophalen huidige gebruiker.", error = ex.Message });
            }
        }
    }
}