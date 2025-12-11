using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.DTO;

namespace backend.Controllers
{
    //Wordt later gebruikt om de code op te schonen.
    public class ControllerMethode
    {

        //public async Task<JsonResult> checkUserValidation([FromBody]KlantDto userDto, ApiContext _context, string rol)
        //{
        //    if(rol == '')
        //    // --- BEGIN BUSINESSLOGICA (REGEL 1: VALIDATIE) ---
        //    if (string.IsNullOrEmpty(userDto.Naam))
        //    {
        //        return new JsonResult(BadRequest("Naam mag niet leeg zijn."));
        //    }

        //    if (string.IsNullOrEmpty(userDto.Email) || !userDto.Email.Contains("@"))
        //    {
        //        return new JsonResult(BadRequest("Een geldig e-mailadres is verplicht."));
        //    }
        //    // --- EINDE BUSINESSLOGICA (REGEL 1) ---


        //    // --- BEGIN BUSINESSLOGICA (REGEL 2: UNIEKE CHECK) ---
        //    // We gebruiken .FirstOrDefault hier i.p.v. .Any om 'case insensitive' te kunnen checken
        //    var emailBestaatAl = _context.Gebruiker
        //        .FirstOrDefault(g => g.Email.ToLower() == userDto.Email.ToLower());

        //    if (emailBestaatAl != null)
        //    {
        //        // Stuur een 'Conflict' (409) foutmelding terug
        //        return new JsonResult(Conflict("Dit e-mailadres is al in gebruik."));
        //    }
        //    // --- EINDE BUSINESSLOGICA (REGEL 2) ---
        //}
    }
}
