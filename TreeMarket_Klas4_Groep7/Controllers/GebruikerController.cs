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

        public GebruikerController(IGebruikerController service)
        {
            _service = service;
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

            // De wachtwoord wordt gehashed binnen de database
            try
            {
                await _service.AddUserAsync(klant, dto.Wachtwoord, "Admin");
                return Ok(new { message = "Klant succesvol geregistreerd!" });
            }
            // Als het mislukt (bijv. wachtwoord te zwak), stuur fouten terug
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        [Authorize(Roles = "Admin")] // Beveiligd met Identity Roles
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            // We gebruiken _context.Users omdat UserManager geen simpele "ToList" heeft voor alle types
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        // DELETE: api/Gebruiker/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
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
            try
            {
                var role = await _service.GetRoleByEmailAsync(email);

                if (role == null)
                    return NotFound(new { message = "Gebruiker niet gevonden." });

                return Ok(new { role });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Er ging iets mis bij het ophalen van de rol." });
            }
        }

    }
}