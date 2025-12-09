using Microsoft.AspNetCore.Mvc;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

//============= NOTE ==============
//De interfaces worden gelinked met de services! Zodat ze in de controller gebruik gemaakt worden.
namespace TreeMarket_Klas4_Groep7.Interfaces
{
    public interface IGebruikerController
    {
        //Kan geen async zetten omdat het de body van de functie nodig heeft.
        Task<Gebruiker> GetByEmailAsync(string email);
        Task AddAsync(Gebruiker gebruiker);
        Task<IEnumerable<Gebruiker>> GetAllAsync();

        //========MAAK HET VOOR NU WEL AAN ================
        Task<Gebruiker> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
