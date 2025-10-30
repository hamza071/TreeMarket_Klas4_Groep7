using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Views;

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

        //Mogelijk voor sub klasses, of het moet apart😝
        [HttpPost]
        public JsonResult CreateGebruiker(Gebruiker gebruiker)
        {
            if (gebruiker.Id == 0) { 
                _context.Gebruiker.Add(gebruiker);
            }
            else
            {
                var gebruikerInDb = _context.Gebruiker.Find(gebruiker.Id);

                if(gebruikerInDb == null) 
                {
                    return new JsonResult(NotFound());
                }
            }
            return new JsonResult(Ok());
        }
    }
}
