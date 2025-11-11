using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.ToDo;

namespace TreeMarket_Klas4_Groep7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private readonly ApiContext _context;

        public GebruikerController(ApiContext context)
        {
            _context = context;
        }

        // --- HIER IS DE BUSINESSLOGICA TOEGEVOEGD ---
        // Maakt een klant aan met business-regels
        [HttpPost] // Er was maar één [HttpPost] nodig voor het aanmaken
        public JsonResult CreateUserKlantTest([FromBody] KlantToDo klantToDo)
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
                // GebruikerId wordt automatisch ingevuld door de database,
                // die hoef je niet vanuit de ToDo (DTO) mee te geven.
                Naam = klantToDo.Naam,
                Email = klantToDo.Email,
                Telefoonnummer = klantToDo.Telefoonnummer,
                Wachtwoord = klantToDo.Wachtwoord, 
                // Rol wordt automatisch "Klant" door constructor
            };

            // --- BEGIN FOUTAFHANDELING (DATABASE) ---
            try
            {
                _context.Gebruiker.Add(klant);
                _context.SaveChanges(); // Sla de wijzigingen op
            }
            catch (Exception ex)
            {
                // Vang eventuele databasefouten af
                // Log de 'ex' variabele als je een logging-systeem hebt
                return new JsonResult(StatusCode(500, "Er is een interne serverfout opgetreden bij het opslaan."));
            }
            // --- EINDE FOUTAFHANDELING ---

            // Stuur het nieuwe 'klant' object terug (nu met een database ID)
            return new JsonResult(Ok(klant));
        }


        // --- ROUTE CORRECTIE ---
        // Toont alle gebruikers
        // De '/' aan het begin is weggehaald, anders negeert het "api/[controller]"
        [HttpGet("All")] // Was [HttpGet("/GetAll")]
        public JsonResult GetAllUsers()
        {
            var result = _context.Gebruiker.ToList();

            return new JsonResult(Ok(result));
        }

        //Verwijderd de gebruiker uit de database
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var result = _context.Gebruiker.Find(id);

            if (result == null)
                return new JsonResult(NotFound());

            _context.Gebruiker.Remove(result);
            _context.SaveChanges();

            return new JsonResult(NoContent());
        }

        // --- ROUTE CORRECTIE ---
        // Je had twee 'Get op ID' methodes. 
        // Dit is de standaard manier, dus de andere (GetUserById) kan weg.
        // Ik heb het omgedoopt van 'GetUserWithChildrenEager' naar 'GetUserById'
        [HttpGet("{id}")]
        public JsonResult GetUserById(int id)
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
    }
}