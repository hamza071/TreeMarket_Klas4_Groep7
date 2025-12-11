using TreeMarket_Klas4_Groep7.Models;

namespace backend.Interfaces
{
    public interface ILeverancierController
    {
        Task<IEnumerable<Leverancier>> GetAllAsync();
        Task<Leverancier> GetByIdAsync(string id);
        Task<bool> DeleteAsync(string id);

    }
}
