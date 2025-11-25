using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;

namespace TreeMarket_Klas4_Groep7.Services
{
    //IT service desk voor de gebruikersController. 
    //Als de gebruikersController iets nodig heeft, kan hij het hier ophalen.
    public class GebruikerService
    {
        //Context wordt zowel bij controller als service gebruikt.
        private readonly ApiContext _context;

        public GebruikerService(ApiContext context)
        {
            _context = context;
        }

        //Deze methode controleerd of de mail bestaat of niet.
        public async Task<bool> EmailBestaatAl(string email)
        {
            return await _context.Gebruiker.AnyAsync(g => g.Email.ToLower() == email.ToLower());
        }
    }
}
