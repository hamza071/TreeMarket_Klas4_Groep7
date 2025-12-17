using System.Collections.Generic;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace backend.Interfaces
{
    public interface IVeilingController
    {
        Task<Veiling> CreateVeilingAsync(VeilingDto dto, string userId);
        
        Task<List<Veiling>> GetAllAsync();
        Task<Veiling> GetByIdAsync(int veilingId);
        Task<Bid> PlaceBidAsync(CreateBidDTO dto, string userId);
        Task<Veiling> UpdateStatusAsync(int veilingId, bool status);
    }
}