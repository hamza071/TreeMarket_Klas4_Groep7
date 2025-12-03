using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models; // Zorg dat deze erbij staat voor 'Gebruiker'

namespace TreeMarket_Klas4_Groep7.Services
{
    public class GebruikerService
    {
        private readonly ApiContext _context;

        public GebruikerService(ApiContext context)
        {
            _context = context;
        }

        // Deze methode controleert of de mail bestaat of niet.
        public async Task<bool> EmailBestaatAl(string email)
        {
            // AANGEPAST: .Gebruiker -> .Users
            // IdentityDbContext noemt de tabel met gebruikers altijd 'Users'
            return await _context.Users.AnyAsync(g => g.Email.ToLower() == email.ToLower());
        }
    }
}