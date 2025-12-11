using backend.Models;
using backend.Models.DTO;

namespace backend.Interfaces
{
    public interface IProductController
    {
        Task<List<ProductDto>> GetProductenVanVandaagAsync();
        Task<List<ProductMetLeverancierDto>> GetProductenMetLeverancierAsync();
        Task<Product> AddOrUpdateProductAsync(Product product);
        Task<Product> GetByIdAsync(int productId);
        Task<bool> DeleteAsync(int productId);
    }
}
