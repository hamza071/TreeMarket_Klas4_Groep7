using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Views;

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
        public JsonResult CreateUser(Gebruiker gebruiker)
        {
            if(gebruiker.GebruikerId == 0)
            {
                //Dit gaat een gebruiker aanmaken
                _context.Gebruiker.Add(gebruiker);
            } else
            {
                var gebruikerInDb = _context.Gebruiker.Find(gebruiker.GebruikerId);

                if(gebruikerInDb == null)
                {
                    return new JsonResult(NotFound());
                }

                gebruikerInDb = gebruiker;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(gebruiker));
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
    }
}
