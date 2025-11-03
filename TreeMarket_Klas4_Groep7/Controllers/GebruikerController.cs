using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Views;
using TreeMarket_Klas4_Groep7.Models;

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

        //Een gebruiker aanmaken
        [HttpPost]
        //Wil dit later de datatype klasse 'KlantToDo' gebruiken zodat je niet veel te veel in Post hoeft in te vullen.
        public JsonResult CreateUserKlant(Gebruiker klant)
        {
            if(klant.GebruikerId == 0)
            {
                //Dit gaat een gebruiker aanmaken
                _context.Gebruiker.Add(klant);
            } else
            {
                var gebruikerInDb = _context.Gebruiker.Find(klant.GebruikerId);

                if(gebruikerInDb == null)
                {
                    return new JsonResult(NotFound());
                }

                gebruikerInDb = klant;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(klant));
        }

        //Deze methode toont alle gebruikers
        [HttpGet]
        public JsonResult GetUserById(int id)
        {
            var result = _context.Gebruiker.Find(id);
            if(result == null)
            {
                return new JsonResult(NotFound("Id is not found: " + id));
            }
            return new JsonResult(Ok(result));
        }

        //Toont alle gebruikers
        [HttpGet("/GetAll")]
        public JsonResult GetAllUsers()
        {
            var result = _context.Gebruiker.ToList();

            return new JsonResult(Ok(result));
        }

        //Verwijderd de gebruiker uit de database
        //De id binnen de HttpDelete zorgt ervoor dat het verplicht is om dat te vullen. Zonder id, gaat het niet werken.
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



        //_______Eager loading toegepast________
        [HttpGet("{id}")]
        public JsonResult GetUserWithChildrenEager(int id)
        {
            //Eager loading, omdat het gelijk de waarde binnen de 'include' gaat toevoegen

            var gebruiker = _context.Gebruiker
                //Include is eager loading
                //Het haalt meteen de child klasses op en toont de waardes
                .Include(gebruiker => gebruiker.Klant)
                .Include(gebruiker => gebruiker.Leverancier)
                .Include(gebruiker => gebruiker.Veilingsmeester)
                .FirstOrDefault(gebruiker => gebruiker.GebruikerId == id);

            if (gebruiker == null) return new JsonResult(NotFound());

            return new JsonResult(Ok("Sigma boy!"));
        }
    }
}
