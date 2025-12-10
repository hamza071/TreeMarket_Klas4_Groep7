using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private readonly IGebruikerController _service;

        // We injecteren UserManager (voor logica) en ApiContext (voor lijstjes ophalen)
        public GebruikerController(UserManager<Gebruiker> userManager, ApiContext context)
        {
            _service = service;
        }

        // ================= REGISTRATIE ENDPOINTS =================

        [HttpPost("Klant")]
        public async Task<IActionResult> CreateUserKlant([FromBody] KlantDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var klant = new Klant
            {
                UserName = dto.Email,
                Email = dto.Email,
                Naam = dto.Naam,
                PhoneNumber = dto.Telefoonnummer,
                EmailConfirmed = true
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

            try
            {
                await _service.AddUserAsync(leverancier, dto.Wachtwoord, "Leverancier");
                return Ok(new { message = "Leverancier succesvol geregistreerd!" });
            }
            // Als het mislukt (bijv. wachtwoord te zwak), stuur fouten terug
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            try
            {
                await _service.AddUserAsync(vm, dto.Wachtwoord, "Veilingsmeester");
                return Ok(new { message = "Veilingsmeester succesvol geregistreerd!" });
            }
            // Als het mislukt (bijv. wachtwoord te zwak), stuur fouten terug
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================= BEHEER FUNCTIES (Alleen voor Admin) =================

        // GET: api/Gebruiker/GetAllUsers
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Gebruiker niet gevonden.");

            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound("Gebruiker niet gevonden.");
            
            return Ok(user);
        }

        // ================= NIEUW: ROL OPHALEN OP BASIS VAN EMAIL =================

        // GET: api/Gebruiker/RoleByEmail?email=...
        [HttpGet("RoleByEmail")]
        public async Task<IActionResult> GetRoleByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is verplicht." });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "Gebruiker niet gevonden." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault(); // Klant / Leverancier / Veilingsmeester

            return Ok(new { role });
        }
    }
}
