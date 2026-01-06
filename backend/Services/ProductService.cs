using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly ApiContext _context;

    public ProductService(ApiContext context)
    {
        _context = context;
    }

    public async Task<List<ProductMetVeilingmeesterDto>> GetProductenVanVandaagAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _context.Product
            .Include(p => p.Leverancier)
            .Where(p => p.Dagdatum.Date == today)
            .Select(p => new ProductMetVeilingmeesterDto
            {
                ProductId = p.ProductId,
                Naam = p.ProductNaam,
                Varieteit = p.Varieteit,
                Omschrijving = p.Omschrijving,
                Hoeveelheid = p.Hoeveelheid,
                MinimumPrijs = p.MinimumPrijs,
                Foto = p.Foto,
                Status = "pending",
                LeverancierNaam = p.Leverancier!.Bedrijf
            })
            .ToListAsync();
    }

    public async Task<List<ProductMetVeilingmeesterDto>> GetProductenMetLeverancierAsync()
    {
        return await _context.Product
            .Include(p => p.Leverancier)
            .Select(p => new ProductMetVeilingmeesterDto
            {
                ProductId = p.ProductId,
                Naam = p.ProductNaam,
                MinimumPrijs = p.MinimumPrijs,
                LeverancierNaam = p.Leverancier!.Bedrijf
            })
            .ToListAsync();
    }

    [Authorize(Roles = "Leverancier,Admin")]
    public async Task<Product> CreateProductAsync(ProductUploadDto dto, string userId, bool isAdmin)
    {
        string leverancierId = userId;

        if (!isAdmin)
        {
            var leverancier = await _context.Leverancier
                .FirstOrDefaultAsync(l => l.Id == userId);

            if (leverancier == null)
                throw new InvalidOperationException("Geen leverancier-profiel gevonden.");
        }
        // optioneel: Admin mag product aanmaken zonder leverancier-profiel
        // of met een default leverancier

        // -------- Image upload --------
        string fotoUrl = "/images/default.png";

        if (dto.Foto != null)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images");

            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid()
                + Path.GetExtension(dto.Foto.FileName);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Foto.CopyToAsync(stream);

            fotoUrl = "/images/" + uniqueFileName;
        }

        var product = new Product
        {
            ProductNaam = dto.ProductNaam,
            Varieteit = dto.Varieteit ?? "Onbekend",
            Omschrijving = dto.Omschrijving ?? "Geen omschrijving",
            Hoeveelheid = dto.Hoeveelheid,
            MinimumPrijs = dto.MinimumPrijs,
            Dagdatum = DateTime.UtcNow,
            LeverancierID = leverancierId,
            Foto = fotoUrl
        };

        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }


    public async Task<Product?> AddOrUpdateProductAsync(Product product)
    {
        if (product.ProductId == 0)
            await _context.Product.AddAsync(product);
        else
            _context.Product.Update(product);

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<ProductMetVeilingmeesterDto?> GetByIdAsync(int id)
    {
        return await _context.Product
            .Include(p => p.Leverancier)
            .Where(p => p.ProductId == id)
            .Select(p => new ProductMetVeilingmeesterDto
            {
                ProductId = p.ProductId,
                Naam = p.ProductNaam,
                MinimumPrijs = p.MinimumPrijs,
                LeverancierNaam = p.Leverancier!.Bedrijf
            })
            .FirstOrDefaultAsync();
    }
}
