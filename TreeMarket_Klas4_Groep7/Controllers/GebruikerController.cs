using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Text;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using static TreeMarket_Klas4_Groep7.Models.DTO.KlantDto;
using SC = System.Security.Claims;
using TreeMarket_Klas4_Groep7.Interfaces;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        //De variabele interface moet van de polymorfisme de GebruikerService oproepen
        private readonly IGebruikerController _service;
        private readonly PasswordHasher<Gebruiker> _passwordHasher;
        private readonly IConfiguration _configuration;

        public GebruikerController(IGebruikerController service, PasswordHasher<Gebruiker> passwordHasher, IConfiguration configuration)
        {
            _service = service;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // ================== ENTITY -> RESPONSE DTO ==================

        private GebruikerResponseDto MapToGebruikerDto(Gebruiker g)
        {
            if (g == null) return null;

            return new GebruikerResponseDto
            {
                GebruikerId = g.GebruikerId,
                Naam = g.Naam,
                Email = g.Email,
                Rol = g.Rol,
                Telefoonnummer = g.Telefoonnummer
            };
        }

        // ================= REGISTRATIE ENDPOINTS =================

        [HttpPost("Klant")]
        public async Task<IActionResult> CreateUserKlant([FromBody] KlantDto klantToDo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailBestaatAl = await _service.GetByEmailAsync(klantToDo.Email);
            if (emailBestaatAl != null) return Conflict("Dit e-mailadres is al in gebruik.");

            try
            {
                var klant = new Klant
                {
                    Naam = klantToDo.Naam,
                    Email = klantToDo.Email,
                    Telefoonnummer = klantToDo.Telefoonnummer,
                    Rol = "Klant"
                };

                klant.Wachtwoord = _passwordHasher.HashPassword(klant, klantToDo.Wachtwoord);

                await _service.AddAsync(klant);

                return Ok(klant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Klant kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        [HttpPost("Leverancier")]
        public async Task<IActionResult> CreateUserLeverancier([FromBody] LeverancierDto leverancierToDo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailBestaatAl = await _service.GetByEmailAsync(leverancierToDo.Email);
            if (emailBestaatAl != null) return Conflict("Dit e-mailadres is al in gebruik.");

            try
            {
                var leverancier = new Leverancier
                {
                    Naam = leverancierToDo.Naam,
                    Email = leverancierToDo.Email,
                    Telefoonnummer = leverancierToDo.Telefoonnummer,
                    bedrijf = leverancierToDo.Bedrijf,
                    KvKNummer = leverancierToDo.KvKNummer,
                    IBANnummer = leverancierToDo.IBANnummer,
                    Rol = "Leverancier"
                };

                leverancier.Wachtwoord = _passwordHasher.HashPassword(leverancier, leverancierToDo.Wachtwoord);

                await _service.AddAsync(leverancier);

                return Ok(leverancier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Leverancier kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        [HttpPost("Veilingsmeester")]
        public async Task<IActionResult> CreateUserVeilingsmeester([FromBody] VeilingsmeesterDto veilingsmeesterToDo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailBestaatAl = await _service.GetByEmailAsync(veilingsmeesterToDo.Email);
            if (emailBestaatAl != null) return Conflict("Dit e-mailadres is al in gebruik.");

            try
            {
                var veilingsmeester = new Veilingsmeester
                {
                    Naam = veilingsmeesterToDo.Naam,
                    Email = veilingsmeesterToDo.Email,
                    Telefoonnummer = veilingsmeesterToDo.Telefoonnummer,
                    Rol = "Veilingsmeester"
                };

                veilingsmeester.Wachtwoord = _passwordHasher.HashPassword(veilingsmeester, veilingsmeesterToDo.Wachtwoord);

                await _service.AddAsync(veilingsmeester);

                return Ok(veilingsmeester);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veilingmeester kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        // ================= LOGIN FUNCTIE =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var gebruiker = await _service.GetByEmailAsync(loginDto.Email);
                if (gebruiker == null) return Unauthorized("E-mail of wachtwoord is onjuist.");

                var result = _passwordHasher.VerifyHashedPassword(gebruiker, gebruiker.Wachtwoord, loginDto.Wachtwoord);

                if (result == PasswordVerificationResult.Failed) return Unauthorized("E-mail of wachtwoord is onjuist.");

                // --- JWT GENERATIE ---
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

                var claims = new List<SC.Claim>
                {
                    new SC.Claim(JwtRegisteredClaimNames.Sub, gebruiker.Email),
                    new SC.Claim("rol", gebruiker.Rol),
                    new SC.Claim(SC.ClaimTypes.Role, gebruiker.Rol ?? "Klant"), // RBAC
                    new SC.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new SC.Claim("GebruikerId", gebruiker.GebruikerId.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Cookie (Veiligheid)
                Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"]))
                });

                // === DE AANPASSING ZIT HIER ===
                // We sturen het token nu mee in de JSON body, zodat React het kan opslaan!
                return Ok(new 
                { 
                    message = "Login succesvol.", 
                    token = tokenString, // <--- DIT IS WAT JE MISTE
                    rol = gebruiker.Rol,
                    gebruikerId = gebruiker.GebruikerId
                });
                // ==============================
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Login is mislukt.", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwtToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            return Ok(new { message = "Uitloggen succesvol." });
        }

        // ================= BEVEILIGDE ROUTES (RBAC) =================


        [Authorize(Roles = "Admin")] // Alleen Admin mag alles zien
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null) return NotFound("Id is not found: " + id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")] // Alleen Admin mag verwijderen
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
    }
}