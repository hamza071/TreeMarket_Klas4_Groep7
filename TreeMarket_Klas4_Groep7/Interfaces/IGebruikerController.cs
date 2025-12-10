using TreeMarket_Klas4_Groep7.Models;

//============= NOTE ==============
//De interfaces worden gelinked met de services! Zodat ze in de controller gebruik gemaakt worden.
namespace backend.Interfaces
{
    public interface IGebruikerController
    {
        //Kan geen async zetten omdat het de body van de functie nodig heeft.
        Task<Gebruiker> GetByEmailAsync(string email);
        Task AddUserAsync(Gebruiker gebruiker, string wachtwoord, string role);
        Task<IEnumerable<Gebruiker>> GetAllAsync();
        Task<Gebruiker> GetByIdAsync(string id);
        Task DeleteAsync(string id);
        Task<bool> EmailBestaatAl(string email);
        Task<string?> GetRoleByEmailAsync(string email);

    }
}
