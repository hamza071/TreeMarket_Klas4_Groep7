using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Interfaces;

namespace TreeMarket_Klas4_Groep7.Services
{
    // ✅ Service class voor Product-logica
    // Houdt LINQ-query’s netjes apart van de controller
    public class ProductService : IProductController
    {
        //Maakt gebruik van de ApiContext
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context; // DbContext injectie
        }

        // ✅ Haal alle producten van vandaag op
        public async Task<List<ProductDto>> GetProductenVanVandaag()
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
        public async Task<List<ProductMetLeverancierDto>> GetProductenMetLeverancier()
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
        public async Task<Product> AddOrUpdateProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                // Nieuw product
                await _context.Product.AddAsync(product);
            }
            else
            {
                // Bestaand product updaten
                var productInDb = _context.Product.Find(product.ProductId);
                if (productInDb != null)
                {
                    productInDb.Foto = product.Foto;
                    productInDb.Artikelkenmerken = product.Artikelkenmerken;
                    productInDb.Hoeveelheid = product.Hoeveelheid;
                    productInDb.MinimumPrijs = product.MinimumPrijs;
                    productInDb.Dagdatum = product.Dagdatum;
                    productInDb.LeverancierID = product.LeverancierID;
                }
            }

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Product.AddAsync(product);
            await _context.SaveChangesAsync();
        }

    }
}