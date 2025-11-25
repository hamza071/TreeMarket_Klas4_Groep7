using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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

        public GebruikerController(ApiContext context, PasswordHasher<Gebruiker> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

            var emailBestaatAl = _context.Gebruiker
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

                return (Ok(klant));
            }
            catch (Exception ex)
            {
                return new JsonResult(StatusCode(500, new { message = "Databasefout: Klant kan niet aangemaakt worden.", error = ex.Message }));
            }

        }

        //Maakt een leverancier aan
        [HttpPost("Leverancier")]
        public async Task<IActionResult> CreateUserLeverancier(LeverancierDto leverancierToDo)
        {
            //Pas de code van de annotaties binnen de Gebruiker controller toe
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == leverancierToDo.Email.ToLower());

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

                leverancier.Wachtwoord = _passwordHasher.HashPassword(leverancier, leverancierToDo.Wachtwoord);


                await _context.Gebruiker.AddAsync(leverancier);
                await _context.SaveChangesAsync();

                return Ok(leverancier);
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "Databasefout: Leveerancier kan niet aangemaakt worden.", error = ex.Message });
            }
        }

        //Maakt een veilingsmeester aan
        [HttpPost("Veilingsmeester")]
        public async Task<IActionResult> CreateUserVeilingsmeester(VeilingsmeesterDto veilingsmeesterToDo)
        {

            //Pas de code van de annotaties binnen de Gebruiker controller toe
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailBestaatAl = _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == veilingsmeesterToDo.Email.ToLower());

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
                veilingsmeester.Wachtwoord = _passwordHasher.HashPassword(veilingsmeester, veilingsmeesterToDo.Wachtwoord);



                await _context.Gebruiker.AddAsync(veilingsmeester);
                await _context.SaveChangesAsync();

                return Ok(veilingsmeester);
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "Databasefout: Veilingmeester kan niet aangemaakt worden.", error = ex.Message });
            }
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
