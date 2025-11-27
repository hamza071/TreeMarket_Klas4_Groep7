using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using static TreeMarket_Klas4_Groep7.Models.DTO.KlantDto;
using SC = System.Security.Claims;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly PasswordHasher<Gebruiker> _passwordHasher;
        private readonly IConfiguration _configuration;

        public GebruikerController(ApiContext context, PasswordHasher<Gebruiker> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // ================== DTOr-esponse ==================
        
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = await _context.Gebruiker
                .FirstOrDefaultAsync(g => g.Email.ToLower() == klantToDo.Email.ToLower());

            if (emailBestaatAl != null)
                return Conflict("Dit e-mailadres is al in gebruik.");

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

                await _context.Gebruiker.AddAsync(klant);
                await _context.SaveChangesAsync();

                // Alleen DTO teruggeven
                return Ok(MapToGebruikerDto(klant));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Klant kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        [HttpPost("Leverancier")]
        public async Task<IActionResult> CreateUserLeverancier([FromBody] LeverancierDto leverancierToDo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = await _context.Gebruiker
                .FirstOrDefaultAsync(g => g.Email.ToLower() == leverancierToDo.Email.ToLower());

            if (emailBestaatAl != null)
                return Conflict("Dit e-mailadres is al in gebruik.");

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

                await _context.Gebruiker.AddAsync(leverancier);
                await _context.SaveChangesAsync();

                return Ok(MapToGebruikerDto(leverancier));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Leverancier kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        [HttpPost("Veilingsmeester")]
        public async Task<IActionResult> CreateUserVeilingsmeester([FromBody] VeilingsmeesterDto veilingsmeesterToDo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = await _context.Gebruiker
                .FirstOrDefaultAsync(g => g.Email.ToLower() == veilingsmeesterToDo.Email.ToLower());

            if (emailBestaatAl != null)
                return Conflict("Dit e-mailadres is al in gebruik.");

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

                await _context.Gebruiker.AddAsync(veilingsmeester);
                await _context.SaveChangesAsync();

                return Ok(MapToGebruikerDto(veilingsmeester));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veilingsmeester kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        // ================= LOGIN =================

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var gebruiker = await _context.Gebruiker
                    .FirstOrDefaultAsync(g => g.Email.ToLower() == loginDto.Email.ToLower());

                if (gebruiker == null)
                    return Unauthorized("E-mail of wachtwoord is onjuist.");

                var result = _passwordHasher.VerifyHashedPassword(
                    gebruiker,
                    gebruiker.Wachtwoord,
                    loginDto.Wachtwoord
                );

                if (result == PasswordVerificationResult.Failed)
                    return Unauthorized("E-mail of wachtwoord is onjuist.");

                var jwtSettings = _configuration.GetSection("Jwt");

                if (string.IsNullOrEmpty(jwtSettings["Key"]))
                    throw new Exception("Jwt Key is niet geconfigureerd.");

                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

                var claims = new List<SC.Claim>
                {
                    new SC.Claim(JwtRegisteredClaimNames.Sub, gebruiker.Email),
                    new SC.Claim(SC.ClaimTypes.Role, gebruiker.Rol ?? "Klant"),
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

                Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"]))
                });

                var gebruikerDto = MapToGebruikerDto(gebruiker);

                // Vorm blijft compatibel met jouw frontend (token, rol, gebruikerId),
                // plus extra veilige gebruiker-object.
                return Ok(new
                {
                    message = "Login succesvol.",
                    token = tokenString,
                    rol = gebruiker.Rol,
                    gebruikerId = gebruiker.GebruikerId,
                    gebruiker = gebruikerDto
                });
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

        // ================= BEVEILIGDE ENDPOINTS =================

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await _context.Gebruiker.FindAsync(id);
                if (result == null)
                    return NotFound("Id is not found: " + id);

                return Ok(MapToGebruikerDto(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _context.Gebruiker.ToListAsync();
                var dtoList = result.Select(g => MapToGebruikerDto(g));
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _context.Gebruiker.FindAsync(id);
                if (result == null)
                    return NotFound();

                _context.Gebruiker.Remove(result);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout.", error = ex.Message });
            }
        }
    }
}
