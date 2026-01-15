using backend.Data;
using backend.DTO;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.TClaim
{
    public class ClaimMaken
    {
        // claims maken van de gemaakte product
        [Fact]
        public async Task VerwerkAankoop_Maakt_Claim_En_Verlaagt_Voorraad()
        {
            // Arrange: InMemory DB
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "ClaimTestDB")
                .Options;

            using var context = new ApiContext(options);

            // Product Aanmaken door een leverancier
            var product = new Product
            {
                ProductId = 1,
                ProductNaam = "Roos",
                Hoeveelheid = 10,
                Foto = "image.jpg",
                LeverancierID = "leverancier-123", 
                Omschrijving = "Een mooie roos"
            };
            context.Product.Add(product);
            await context.SaveChangesAsync();


            //De mock leverancier die de product gemaakt heeft
            var leverancier = new Leverancier
            {
                Id = "leverancier-123",
                Naam = "Michel Foucault",
                Email = "michel@example.com",
                UserName = "michel",
                PasswordHash = "dummyhash",
                Bedrijf = "Test Leverancier",
                IBANnummer = "NL12BANK345678",
                KvKNummer = "12345678"
            };
            context.Leverancier.Add(leverancier);
            await context.SaveChangesAsync();

            // Veiling aanmaken door een veilingsmeester 
            var veilingsmeester = new Gebruiker
            {
                Id = "veilingsmeester-123",
                Naam = "Joseph Lister",
                Email = "joseph@example.com",
                UserName = "joseph",
                PasswordHash = "dummyhash",
            };
            context.Gebruiker.Add(veilingsmeester);
            await context.SaveChangesAsync();

            var veiling = new Veiling
            {
                VeilingID = 1,
                Product = product,
                Status = true,
                VeilingsmeesterID = "veilingsmeester-123"
            };

            context.Veiling.Add(veiling);
            await context.SaveChangesAsync();

            // De claim wordt gemaakt door klant 

            var service = new ClaimService(context);

            var dto = new ClaimDto
            {
                VeilingId = 1,
                Prijs = 100,
                Aantal = 2
            };

            // Act
            var result = await service.VerwerkAankoopAsync(dto, "klant-123");

            // Assert
            Assert.True(result);

            var claim = await context.Claim.FirstAsync();
            Assert.Equal(100, claim.Prijs);
            Assert.Equal(2, claim.Aantal);
            Assert.Equal("klant-123", claim.KlantId);

            var updatedProduct = await context.Product.FirstAsync();
            Assert.Equal(8, updatedProduct.Hoeveelheid); // 10 - 2
        }
    }
}
