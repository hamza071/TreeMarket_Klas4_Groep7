using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Interfaces
{
    public interface ILeverancierController
    {
        Task<Leverancier> AddAsync(Leverancier leverancier);
        Task<IEnumerable<Leverancier>> GetAllAsync();
        Task<Leverancier?> GetByIdAsync(int id);
    }
}
