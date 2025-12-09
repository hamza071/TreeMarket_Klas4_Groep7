using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Interfaces
{
    public interface IVeilingController
    {
        Task<IEnumerable<Veiling>> GetAllAsync();
        Task<Veiling> GetByIdAsync(int id);
        Task<Veiling> AddAsync(Veiling veiling);
        Task<Veiling> UpdateAsync(Veiling veiling);
        Task<Bid> AddBidAsync(Bid bid);
    }
}
