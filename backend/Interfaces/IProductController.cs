using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

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
