using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.DTO;

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