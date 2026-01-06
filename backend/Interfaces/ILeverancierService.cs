using backend.Models;

namespace backend.Interfaces
{
    public interface ILeverancierService
    {
        Task<IEnumerable<Leverancier>> GetAllAsync();
        Task<Leverancier> GetByIdAsync(string id);
        Task<bool> DeleteAsync(string id);

    }
}
