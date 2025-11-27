using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Text;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using TreeMarket_Klas4_Groep7.Services;
//Niet van de model Claim klasse, maar een library.
//De reden waarom we een alias gebruiken komt omdat er al een model klasse Claim bestaat :)
using SC = System.Security.Claims;


namespace TreeMarket_Klas4_Groep7.Controllers
{
    //Dit is de parent (super) klasse, maar het is vooral voor een test
    //Deze route wordt ook opgenoemd bij de react componenten waar ze de gebruiker data gebruiken
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        //Dit wordt gebruikt via de Database, daarom is de datatype 'readonly'
        private readonly ApiContext _context;
        //Deze variabele wordt opgeroepen binnen de Program.cs
        private readonly PasswordHasher<Gebruiker> _passwordHasher;
        private readonly IConfiguration _configuration;

        public GebruikerController(ApiContext context, PasswordHasher<Gebruiker> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }


        //================De klant, leverancier en veilingsmeester wat in de register front-end moet ==================
        //Er wordt 3 keer de annotatie 'HttpPost' gebruikt.
        //Dus het moet uniek, anders gaat het mis binnen de Swagger.

        //Hier wordt Async toegepast.
        //Async zorgt ervoor dat de uitgevoerde code wacht totdat de vorige klaar is.
        //Het zorgt ervoor dat er niet te veel blockades komt net als de transactie in sql.
        //Asycn zorgt ervoor dat de server niet vastloopt.

        //Maakt een klant aan
        [HttpPost("Klant")] // Er was maar één [HttpPost] nodig voor het aanmaken
        public async Task<IActionResult> CreateUserKlant([FromBody] KlantDto klantToDo)
        {
            //Pas de code van de annotaties binnen de Gebruiker controller toe
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = await _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == klantToDo.Email.ToLower());

            if (emailBestaatAl != null)
                return Conflict("Dit e-mailadres is al in gebruik.");
                        
            try
            {
                var klant = new Klant
                {
                    Naam = klantToDo.Naam,
                    Email = klantToDo.Email,
                    Telefoonnummer = klantToDo.Telefoonnummer,
                    // Rol wordt automatisch "Klant" door constructor in Klant.cs
                };

                //Het wachtwoord wordt gehashed en wordt niet letterlijk opgenomen zoals wat er in het veld staat.
                //Anders is dat wel een security leak💀
                klant.Wachtwoord = _passwordHasher.HashPassword(klant, klantToDo.Wachtwoord);

                await _context.Gebruiker.AddAsync(klant);
                await _context.SaveChangesAsync();

                return Ok(klant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Databasefout: Klant kan niet aangemaakt worden.",
                    error = ex.Message
                });
            }

        }

        //Maakt een leverancier aan
        [HttpPost("Leverancier")]
        public async Task<IActionResult> CreateUserLeverancier([FromBody] LeverancierDto leverancierToDo)
        {
            //Pas de code van de annotaties binnen de Gebruiker controller toe
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
                };

                //Het wachtwoord wordt gehashed en wordt niet letterlijk opgenomen zoals wat er in het veld staat.
                leverancier.Wachtwoord = _passwordHasher.HashPassword(leverancier, leverancierToDo.Wachtwoord);

                await _context.Gebruiker.AddAsync(leverancier);
                await _context.SaveChangesAsync();

                return Ok(leverancier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Leverancier kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        //Maakt een veilingsmeester aan
        [HttpPost("Veilingsmeester")]
        public async Task<IActionResult> CreateUserVeilingsmeester([FromBody] VeilingsmeesterDto veilingsmeesterToDo)
        {
            //Pas de code van de annotaties binnen de Gebruiker controller toe
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
                    // Rol wordt automatisch "Veilingsmeester" door constructor in Klant.cs
                };

                //Het wachtwoord wordt gehashed en wordt niet letterlijk opgenomen zoals wat er in het veld staat.
                veilingsmeester.Wachtwoord = _passwordHasher.HashPassword(veilingsmeester, veilingsmeesterToDo.Wachtwoord);

                await _context.Gebruiker.AddAsync(veilingsmeester);
                await _context.SaveChangesAsync();

                return Ok(veilingsmeester);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Veilingmeester kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        //================ LOGIN FUNCTIE =================
        //Deze functie controleert of de gebruiker kan inloggen op basis van e-mail en wachtwoord
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            //Validatie van de DTO (Required / EmailAddress uit jouw annotaties)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //Zoek gebruiker op basis van e-mail (case-insensitive via ToLower)
                var gebruiker = await _context.Gebruiker
                    .FirstOrDefaultAsync(g => g.Email.ToLower() == loginDto.Email.ToLower());

                //Als geen gebruiker gevonden is → foutmelding
                if (gebruiker == null)
                    return Unauthorized("E-mail of wachtwoord is onjuist.");

                //Vergelijk gehashte wachtwoorden
                var result = _passwordHasher.VerifyHashedPassword(
                    gebruiker,
                    gebruiker.Wachtwoord,    // gehashte wachtwoord uit database
                    loginDto.Wachtwoord      // plain text uit de login
                );

                if (result == PasswordVerificationResult.Failed)
                    return Unauthorized("E-mail of wachtwoord is onjuist.");

                //========== De JWT wordt hier gegenereerd. =========
                //De JWT wordt al opgeroepen in de appsettings.json en Program.cs
                var jwtSettings = _configuration.GetSection("Jwt"); // Of "JwtSettings" als je dat gebruikt
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
                
                //Als iemand de appsettings verpest of vergeet, crasht je app nu op een vage error
                //(bijv. null reference, format exception).
                //Met expliciete checks is het duidelijker waar het misgaat.
                
                if (string.IsNullOrEmpty(jwtSettings["Key"]))
                    throw new Exception("Jwt Key is niet geconfigureerd.");

                var claims = new List<SC.Claim>
                {
                    new SC.Claim(JwtRegisteredClaimNames.Sub, gebruiker.Email), // uniek subject van de JWT
                    new SC.Claim("rol", gebruiker.Rol),  // je eigen claim “rol”
                    new SC.Claim(SC.ClaimTypes.Role, gebruiker.Rol), //Deze code zorgt ervoor dat je de correcte rollen geeft. Bijvoorbeeld voor klant = klant enzo.
                    new SC.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //Unieke token geven
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                //Deze code stuurt ook cookies terug.
                Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, //Om te testen op andere browsers of tabs, is het handig om het op a'false' te zetten. Normaal moet het 'true' zijn
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"]))
                });

                //Als we hier zijn → login is geslaagd
                //Later kun je hier nog een JWT token of rol-informatie aan toevoegen
                return Ok(new { message = "Login succesvol.", gebruiker });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Login is mislukt.", error = ex.Message });
            }
        }

        //================ UITLOGGEN FUNCTIE (moet eerst cookie of JWT token hebben) =================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Cookie verwijderen door de waarde leeg te maken en expiry in het verleden te zetten
            Response.Cookies.Append("jwtToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,            // false voor development
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1) // datum in het verleden
            });

            return Ok(new { message = "Uitloggen succesvol." });
        }



        //--------GET------------
        //Deze methode toont een gebruiker op basis van een ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await _context.Gebruiker.FindAsync(id);
                if (result == null)
                {
                    return NotFound("Id is not found: " + id);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Gebruiker op basis van een ID kan niet opgehaald worden.", error = ex.Message });
            }

        }

        //Toont alle gebruikers
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _context.Gebruiker.ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Gebruikers kan niet opgehaald worden.", error = ex.Message });
            }

        }

        //----------Delete-------------
        //Verwijderd de gebruiker uit de database
        //De id binnen de HttpDelete zorgt ervoor dat het verplicht is om dat te vullen. Zonder id, gaat het niet werken.

        //Deze functie werkt voor alle gebruikers
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _context.Gebruiker.FindAsync(id);

                if (result == null)
                    return NotFound();

                //Bij delete hoef je geen Async te gebruiken omdat het alleen de data verwijderd.
                _context.Gebruiker.Remove(result);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Databasefout: Gebruiker kan niet verwijderd worden.", error = ex.Message });
            }
        }
    }
}
