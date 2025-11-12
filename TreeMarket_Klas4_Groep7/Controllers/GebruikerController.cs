using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.ToDo;


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
        [HttpPost("Klant")]
        public async Task<JsonResult> CreateUserKlantASync(KlantToDo klantToDo)
        {
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
        public async Task<JsonResult> CreateUserLeverancier(LeverancierToDo LeverancierToDo)
        {
            var leverancier = new Leverancier
            {
                Naam = LeverancierToDo.Naam,
                Email = LeverancierToDo.Email,
                Telefoonnummer = LeverancierToDo.Telefoonnummer,
                bedrijf = LeverancierToDo.Bedrijf,
                KvKNummer = LeverancierToDo.KvKNummer,
                IBANnummer = LeverancierToDo.IBANnummer,
                Wachtwoord = LeverancierToDo.Wachtwoord,
                //Rol moet ik nog even kijken.
                // Rol wordt automatisch "Klant" door constructor in Klant.cs
            };

            await _context.Gebruiker.AddAsync(leverancier);
            await _context.SaveChangesAsync();

            return new JsonResult(Ok(leverancier));
        }

        //Maakt een veilingsmeester aan
        [HttpPost("Veilingsmeester")]
        public async Task<JsonResult> CreateUserVeilingsmeester(VeilingsmeesterToDo VeilingsmeesterToDo)
        {
            var veilingsmeester = new Veilingsmeester
            {
                Naam = VeilingsmeesterToDo.Naam,
                Email = VeilingsmeesterToDo.Email,
                Telefoonnummer = VeilingsmeesterToDo.Telefoonnummer,
                PlanDatum = VeilingsmeesterToDo.PlanDatum,
                Wachtwoord = VeilingsmeesterToDo.Wachtwoord,
                //Rol moet ik nog even kijken.
                // Rol wordt automatisch "Klant" door constructor in Klant.cs
            };

            await _context.Gebruiker.AddAsync(veilingsmeester);
            await _context.SaveChangesAsync();

            return new JsonResult(Ok(veilingsmeester));
        }

        //--------GET------------
        //Deze methode toont alle gebruikers
        [HttpGet]
        public async Task<JsonResult> GetUserById(int id)
        {
            var result = await _context.Gebruiker.FindAsync(id);
            if(result == null)
            {
                return new JsonResult(NotFound("Id is not found: " + id));
            }
            return new JsonResult(Ok(result));
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
