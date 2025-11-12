using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    //Dit is de parent (super) klasse, maar het is vooral voor een test
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        //Dit wordt gebruikt via de Database, daarom is de datatype 'readonly'
        private readonly ApiContext _context;

        public GebruikerController(ApiContext context)
        {
            _context = context;
        }

        //Er wordt 3 keer de annotatie 'HttpPost' gebruikt.
        //Dus het moet uniek, anders gaat het mis binnen de Swagger.

        //Hier wordt Async toegepast.
        //Async zorgt ervoor dat de uitgevoerde code wacht totdat de vorige klaar is.
        //Het zorgt ervoor dat er niet te veel blockades komt net als de transactie in sql.
        //Asycn zorgt ervoor dat de server niet vastloopt.

        //Maakt een klant aan

        [HttpPost("Klant")] // Er was maar één [HttpPost] nodig voor het aanmaken
        public async Task<JsonResult> CreateUserKlantTest([FromBody] KlantDto klantToDo)
        {
            // --- BEGIN BUSINESSLOGICA (REGEL 1: VALIDATIE) ---
            if (string.IsNullOrEmpty(klantToDo.Naam))
            {
                return new JsonResult(BadRequest("Naam mag niet leeg zijn."));
            }

            if (string.IsNullOrEmpty(klantToDo.Email) || !klantToDo.Email.Contains("@"))
            {
                return new JsonResult(BadRequest("Een geldig e-mailadres is verplicht."));
            }
            // --- EINDE BUSINESSLOGICA (REGEL 1) ---


            // --- BEGIN BUSINESSLOGICA (REGEL 2: UNIEKE CHECK) ---
            // We gebruiken .FirstOrDefault hier i.p.v. .Any om 'case insensitive' te kunnen checken
            var emailBestaatAl = _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == klantToDo.Email.ToLower());

            if (emailBestaatAl != null)
            {
                // Stuur een 'Conflict' (409) foutmelding terug
                return new JsonResult(Conflict("Dit e-mailadres is al in gebruik."));
            }
            // --- EINDE BUSINESSLOGICA (REGEL 2) ---

            var klant = new Klant
            {
                Naam = klantToDo.Naam,
                Email = klantToDo.Email,
                Telefoonnummer = klantToDo.Telefoonnummer,
                Wachtwoord = klantToDo.Wachtwoord,
                // Rol wordt automatisch "Klant" door constructor in Klant.cs
            };

            await _context.Gebruiker.AddAsync(klant);
            await _context.SaveChangesAsync();

            return new JsonResult(Ok(klant));
        }

        //Maakt een leverancier aan
        [HttpPost("Leverancier")]
        public async Task<JsonResult> CreateUserLeverancier(LeverancierDto leverancierToDo)
        {

            // --- BEGIN BUSINESSLOGICA (REGEL 1: VALIDATIE) ---
            if (string.IsNullOrEmpty(leverancierToDo.Naam))
            {
                return new JsonResult(BadRequest("Naam mag niet leeg zijn."));
            }

            if (string.IsNullOrEmpty(leverancierToDo.Email) || !leverancierToDo.Email.Contains("@"))
            {
                return new JsonResult(BadRequest("Een geldig e-mailadres is verplicht."));
            }
            // --- EINDE BUSINESSLOGICA (REGEL 1) ---


            // --- BEGIN BUSINESSLOGICA (REGEL 2: UNIEKE CHECK) ---
            // We gebruiken .FirstOrDefault hier i.p.v. .Any om 'case insensitive' te kunnen checken
            var emailBestaatAl = _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == leverancierToDo.Email.ToLower());

            if (emailBestaatAl != null)
            {
                // Stuur een 'Conflict' (409) foutmelding terug
                return new JsonResult(Conflict("Dit e-mailadres is al in gebruik."));
            }
            // --- EINDE BUSINESSLOGICA (REGEL 2) ---


            {
                var leverancier = new Leverancier
                {
                    Naam = leverancierToDo.Naam,
                    Email = leverancierToDo.Email,
                    Telefoonnummer = leverancierToDo.Telefoonnummer,
                    bedrijf = leverancierToDo.Bedrijf,
                    KvKNummer = leverancierToDo.KvKNummer,
                    IBANnummer = leverancierToDo.IBANnummer,
                    Wachtwoord = leverancierToDo.Wachtwoord,
                    //Rol moet ik nog even kijken.
                    // Rol wordt automatisch "Klant" door constructor in Klant.cs
                };

                await _context.Gebruiker.AddAsync(leverancier);
                await _context.SaveChangesAsync();

                return new JsonResult(Ok(leverancier));
            }
        }

        //Maakt een veilingsmeester aan
        [HttpPost("Veilingsmeester")]
        public async Task<JsonResult> CreateUserVeilingsmeester(VeilingsmeesterDto veilingsmeesterToDo)
        {


            // --- BEGIN BUSINESSLOGICA (REGEL 1: VALIDATIE) ---
            if (string.IsNullOrEmpty(veilingsmeesterToDo.Naam))
            {
                return new JsonResult(BadRequest("Naam mag niet leeg zijn."));
            }

            if (string.IsNullOrEmpty(veilingsmeesterToDo.Email) || !veilingsmeesterToDo.Email.Contains("@"))
            {
                return new JsonResult(BadRequest("Een geldig e-mailadres is verplicht."));
            }
            // --- EINDE BUSINESSLOGICA (REGEL 1) ---


            // --- BEGIN BUSINESSLOGICA (REGEL 2: UNIEKE CHECK) ---
            // We gebruiken .FirstOrDefault hier i.p.v. .Any om 'case insensitive' te kunnen checken
            var emailBestaatAl = _context.Gebruiker
                .FirstOrDefault(g => g.Email.ToLower() == veilingsmeesterToDo.Email.ToLower());

            if (emailBestaatAl != null)
            {
                // Stuur een 'Conflict' (409) foutmelding terug
                return new JsonResult(Conflict("Dit e-mailadres is al in gebruik."));
            }

            // --- EINDE BUSINESSLOGICA (REGEL 2) ---
            {
                var veilingsmeester = new Veilingsmeester
                {
                    Naam = veilingsmeesterToDo.Naam,
                    Email = veilingsmeesterToDo.Email,
                    Telefoonnummer = veilingsmeesterToDo.Telefoonnummer,
                    PlanDatum = veilingsmeesterToDo.PlanDatum,
                    Wachtwoord = veilingsmeesterToDo.Wachtwoord,
                    //Rol moet ik nog even kijken.
                    // Rol wordt automatisch "Klant" door constructor in Klant.cs
                };

                await _context.Gebruiker.AddAsync(veilingsmeester);
                await _context.SaveChangesAsync();

                return new JsonResult(Ok(veilingsmeester));
            }
        }

        //--------GET------------
        //Deze methode toont alle gebruikers
        [HttpGet]
        public async Task<JsonResult> GetUserById(int id)
        {
            //Eager loading, omdat het gelijk de waarde binnen de 'include' gaat toevoegen
            var gebruiker = _context.Gebruiker
                .Include(gebruiker => gebruiker.Klant)
                .Include(gebruiker => gebruiker.Leverancier)
                .Include(gebruiker => gebruiker.Veilingsmeester)
                .FirstOrDefault(gebruiker => gebruiker.GebruikerId == id);

            if (gebruiker == null) 
            {
                return new JsonResult(NotFound($"Gebruiker met id {id} niet gevonden."));
            }
            
            // Stuur de gevonden gebruiker terug, niet de tekst "Sigma boy!"
            return new JsonResult(Ok(gebruiker));
        }

        //Toont alle gebruikers
        [HttpGet("/GetAll")]
        public async Task<JsonResult> GetAllUsers()
        {
            var result = await _context.Gebruiker.ToListAsync();

            return new JsonResult(Ok(result));
        }

        //----------Delete-------------
        //Verwijderd de gebruiker uit de database
        //De id binnen de HttpDelete zorgt ervoor dat het verplicht is om dat te vullen. Zonder id, gaat het niet werken.

        //Deze functie werkt voor alle gebruikers
        [HttpDelete("{id}")]
        public async Task<JsonResult> DeleteUser(int id)
        {
            var result = await _context.Gebruiker.FindAsync(id);

            if (result == null)
                return new JsonResult(NotFound());

            //Bij delete hoef je geen Async te gebruiken omdat het alleen de data verwijderd.
            _context.Gebruiker.Remove(result);
            await _context.SaveChangesAsync();

            return new JsonResult(NoContent());
        }



        ////_______Eager loading toegepast________
        //[HttpGet("{id}")]
        //public async Task<JsonResult> GetUserWithChildrenEager(int id)
        //{
        //    //Eager loading, omdat het gelijk de waarde binnen de 'include' gaat toevoegen

        //    var gebruiker = await _context.Gebruiker
        //        //Include is eager loading
        //        //Het haalt meteen de child klasses op en toont de waardes
        //        .Include(gebruiker => gebruiker.Klant)
        //        .Include(gebruiker => gebruiker.Leverancier)
        //        .Include(gebruiker => gebruiker.Veilingsmeester)
        //        .FirstOrDefaultAsync(gebruiker => gebruiker.GebruikerId == id);

        //    if (gebruiker == null) return new JsonResult(NotFound());

        //    return new JsonResult(Ok("Sigma boy!"));
        //}
    }
}
