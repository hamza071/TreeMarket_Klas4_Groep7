using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Interfaces
{
    public interface IProductController
    {
        Task<List<ProductDto>> GetProductenVanVandaag();
        Task<List<ProductMetLeverancierDto>> GetProductenMetLeverancier();
        Task<Product> AddOrUpdateProduct(Product product);
        Task AddAsync(Product product);
    }
}
