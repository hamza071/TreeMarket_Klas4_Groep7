using backend.Models;

namespace backend.Interfaces
{
    public interface IDashboardController
    {
        Task<IEnumerable<Dashboard>> GetAllAsync();
        Task<Dashboard?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Dashboard dashboard);
        Task<Dashboard> AddAsync(Dashboard dashboard);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}