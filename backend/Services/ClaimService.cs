using backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTO;
using Microsoft.Data.SqlClient; 
using System.Data;

namespace backend.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApiContext _context;

        public ClaimService(ApiContext context)
        {
            _context = context;
        }

        // De gewone methodes mogen vaak wel EF blijven (tenzij je ALLES om moet bouwen)
        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await _context.Claim
                .Include(c => c.klant)
                .Include(c => c.Veiling)
                    .ThenInclude(v => v.Product)
                .ToListAsync();
        }

        public async Task<bool> VerwerkAankoopAsync(ClaimDto dto, string userId)
        {
            var veiling = await _context.Veiling.Include(v => v.Product).FirstOrDefaultAsync(v => v.VeilingID == dto.VeilingId);
            if (veiling == null || veiling.Product == null) throw new Exception("Veiling/Product niet gevonden.");
            if (veiling.Status != true) throw new Exception("Veiling gesloten.");
            if (veiling.Product.Hoeveelheid < dto.Aantal) throw new Exception("Te weinig voorraad.");

            var nieuweClaim = new Claim
            {
                KlantId = userId,
                VeilingId = dto.VeilingId,
                Prijs = dto.Prijs,
                Aantal = dto.Aantal
            };

            _context.Claim.Add(nieuweClaim);
            veiling.Product.Hoeveelheid -= dto.Aantal;
            if (veiling.Product.Hoeveelheid <= 0) { veiling.Product.Hoeveelheid = 0; veiling.Status = false; }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim == null) return false;
            _context.Claim.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        // HIERONDER IS HET 'RAW SQL' GEDEELTE VOOR DE HISTORIE (ZONDER EF)
        public async Task<ProductHistoryResponse> GetHistoryAsync(string productNaam, string leverancierNaam)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            // 1. MARKT HISTORIE
            var marktLijst = new List<HistoryDto>();
            string sqlMarkt = @"
                SELECT TOP 10 c.Prijs, v.StartTimestamp, l.Bedrijf
                FROM Claim c
                JOIN Veiling v ON c.VeilingId = v.VeilingID
                JOIN Product p ON v.ProductID = p.ProductId   -- <--- HIER ZAT DE FOUT (Was ProductProductId)
                JOIN Leverancier l ON p.LeverancierID = l.Id
                WHERE p.ProductNaam = @Naam
                ORDER BY v.StartTimestamp DESC";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlMarkt;
                var p = cmd.CreateParameter(); p.ParameterName = "@Naam"; p.Value = productNaam; cmd.Parameters.Add(p);
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        marktLijst.Add(new HistoryDto {
                            Prijs = reader.GetDecimal(0),
                            Datum = reader.GetDateTime(1).ToString("dd MMMM yyyy"),
                            Aanbieder = reader.GetString(2)
                        });
                    }
                }
            }

            // 2. EIGEN HISTORIE
            var eigenLijst = new List<HistoryDto>();
            string sqlEigen = @"
                SELECT TOP 10 c.Prijs, v.StartTimestamp, l.Bedrijf
                FROM Claim c
                JOIN Veiling v ON c.VeilingId = v.VeilingID
                JOIN Product p ON v.ProductID = p.ProductId   -- <--- OOK HIER AANGEPAST
                JOIN Leverancier l ON p.LeverancierID = l.Id
                WHERE p.ProductNaam = @Naam AND l.Bedrijf = @LevNaam
                ORDER BY v.StartTimestamp DESC";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlEigen;
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@Naam"; p1.Value = productNaam; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@LevNaam"; p2.Value = leverancierNaam; cmd.Parameters.Add(p2);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        eigenLijst.Add(new HistoryDto {
                            Prijs = reader.GetDecimal(0),
                            Datum = reader.GetDateTime(1).ToString("dd MMMM yyyy"),
                            Aanbieder = reader.GetString(2)
                        });
                    }
                }
            }

            return new ProductHistoryResponse
            {
                MarktHistorie = marktLijst.Take(10).ToList(),
                GemiddeldeMarkt = marktLijst.Any() ? marktLijst.Average(x => x.Prijs) : 0,
                EigenHistorie = eigenLijst.Take(10).ToList(),
                GemiddeldeEigen = eigenLijst.Any() ? eigenLijst.Average(x => x.Prijs) : 0
            };
        }
    }
}