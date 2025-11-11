using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace TreeMarket_Klas4_Groep7.Services
{
    // ✅ Service class voor Product-logica
    // Houdt LINQ-query’s netjes apart van de controller
    public class ProductService
    {
        private readonly ApiContext _context;

        public ProductService(ApiContext context)
        {
            _context = context; // DbContext injectie
        }

        // ✅ Haal alle producten van vandaag op
        public List<ProductDto> GetProductenVanVandaag()
        {
            var vandaag = DateTime.Today;

            return _context.Producten
                .Where(p => p.Dagdatum.Date == vandaag)  // Filter: alleen producten van vandaag
                .OrderBy(p => p.MinimumPrijs)           // Sorteer op minimumprijs
                .Select(p => new ProductDto             // Projecteer naar DTO
                {
                    ProductId = p.ProductId,
                    Foto = p.Foto,
                    MinimumPrijs = p.MinimumPrijs,
                    Hoeveelheid = p.Hoeveelheid
                })
                .ToList(); // Voer query uit
        }

        // ✅ Haal producten op met Leverancier info
        public List<ProductMetLeverancierDto> GetProductenMetLeverancier()
        {
            return _context.Producten
                .Include(p => p.Leverancier) // Zorg dat Leverancier geladen wordt
                .Select(p => new ProductMetLeverancierDto
                {
                    ProductId = p.ProductId,
                    MinimumPrijs = p.MinimumPrijs,
                    LeverancierNaam = p.Leverancier.Naam
                })
                .ToList();
        }

        // ✅ Voeg een nieuw product toe of update een bestaand product
        public Product AddOrUpdateProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                // Nieuw product
                _context.Producten.Add(product);
            }
            else
            {
                // Bestaand product updaten
                var productInDb = _context.Producten.Find(product.ProductId);
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

            _context.SaveChanges();
            return product;
        }
    }
}