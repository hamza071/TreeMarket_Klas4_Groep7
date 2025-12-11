using backend.Data;
using backend.Models;
using backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using backend.Interfaces;

namespace backend.Services
{
    // ✅ Service class voor Product-logica
    // Houdt LINQ-query’s netjes apart van de controller
    public class ProductService: IProductController
    {
        //Maakt gebruik van de ApiContext
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context; // DbContext injectie
        }

        // ✅ Haal alle producten van vandaag op
        public async Task<List<ProductDto>> GetProductenVanVandaagAsync()
        {
            var vandaag = DateTime.Today;

            return await _context.Product
                .Where(p => p.Dagdatum.Date == vandaag)  // Filter: alleen producten van vandaag
                .OrderBy(p => p.MinimumPrijs)           // Sorteer op minimumprijs
                .Select(p => new ProductDto             // Projecteer naar DTO
                {
                    ProductId = p.ProductId,
                    Foto = p.Foto,
                    MinimumPrijs = p.MinimumPrijs,
                    Hoeveelheid = p.Hoeveelheid
                })
                .ToListAsync(); // Voer query uit
        }

        // ✅ Haal producten op met Leverancier info
        public async Task<List<ProductMetLeverancierDto>> GetProductenMetLeverancierAsync()
        {
            return await _context.Product
                .Include(p => p.Leverancier) // Zorg dat Leverancier geladen wordt
                .Select(p => new ProductMetLeverancierDto
                {
                    ProductId = p.ProductId,
                    MinimumPrijs = p.MinimumPrijs,
                    LeverancierNaam = p.Leverancier.Naam
                })
                .ToListAsync();
        }

        // ✅ Voeg een nieuw product toe of update een bestaand product
        public async Task<Product?> AddOrUpdateProductAsync(Product product)
        {
            // ✅ Validatie
            if (string.IsNullOrWhiteSpace(product.Artikelkenmerken)) return null;
            if (product.Hoeveelheid <= 0) return null;
            if (product.MinimumPrijs < 0) return null;
            if (product.Dagdatum.Date < DateTime.Today) return null;
            if (string.IsNullOrEmpty(product.LeverancierID)) return null;

            // Add or update
            if (product.ProductId == 0)
            {
                await _context.Product.AddAsync(product);
            }
            else
            {
                var productInDb = await _context.Product.FindAsync(product.ProductId);
                if (productInDb != null)
                {
                    productInDb.Foto = product.Foto;
                    productInDb.Artikelkenmerken = product.Artikelkenmerken;
                    productInDb.Hoeveelheid = product.Hoeveelheid;
                    productInDb.MinimumPrijs = product.MinimumPrijs;
                    productInDb.Dagdatum = product.Dagdatum;
                    productInDb.LeverancierID = product.LeverancierID;
                }
                else
                {
                    return null; // Product niet gevonden
                }
            }

            await _context.SaveChangesAsync();
            return product;
        }


        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _context.Product.FindAsync(productId);
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return false;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}