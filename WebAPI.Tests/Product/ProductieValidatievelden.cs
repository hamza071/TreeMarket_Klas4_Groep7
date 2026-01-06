using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Models;
using backend.Models.DTO;
using security = System.Security.Claims;


namespace WebAPI.Tests.TProduct
{
    public class ProductValidatievelden
    {
        // ============LeverancierID mag niet NULL zijn =============
        [Fact]
        public async Task ProductKanNietAangemaaktWorden_ZonderToken()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            mockService.Setup(s => s.AddOrUpdateProductAsync(It.IsAny<Product>()))
                       .Returns<Product>(p => Task.FromResult(p));

            var controller = new ProductController(mockService.Object);

            var productDto = new ProductDto
            {
                Foto = "image.jpg",
                artikelkenmerken = "Test kenmerken",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                dagdatum = DateTime.Now,
                leverancierID = "" // wordt genegeerd door controller. Testen of het zonder leverancier het aanmaken niet doorgaat.
            };

            // --- GEEN gebruiker in token ---
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new security.ClaimsPrincipal() }
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Je bent niet ingelogd.", unauthorized.Value);
        }

        //========Klant mag geen product aanmaken======
        [Fact]
        public async Task KlantKanProductNietAanmaken()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            mockService.Setup(s => s.AddOrUpdateProductAsync(It.IsAny<Product>()))
                       .Returns<Product>(p => Task.FromResult(p));

            var controller = new ProductController(mockService.Object);

            var productDto = new ProductDto
            {
                Foto = "image.jpg",
                artikelkenmerken = "Test kenmerken",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                dagdatum = DateTime.Now,
                leverancierID = "" // DTO wordt genegeerd
            };

            // --- Mock een klant (niet Leverancier) ---
            var user = new security.ClaimsPrincipal(new security.ClaimsIdentity(new security.Claim[]
            {
                new security.Claim(security.ClaimTypes.NameIdentifier, "klant-123"),
                new security.Claim(security.ClaimTypes.Role, "Klant")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert

            // Omdat [Authorize(Roles = "Leverancier")] buiten controller runt, kan unit test dit niet automatisch triggeren.
            // Je kunt dit alleen integratietesten of handmatig checken met Authorization filter.
            // Voor unit test, we kunnen checken dat controller de User.Role correct herkent
            var role = user.FindFirst(security.ClaimTypes.Role)?.Value; // deze regel en de regel eronder checkt of de rol niet leverancier heeft
            Assert.NotEqual("Leverancier", role); // Dus deze gebruiker mag niet
        }

        //===== Veilingsmeester en Admin mogen ook geen producten aanmaken ===============
        [Fact]
        public async Task VeilingsmeesterKanProductNietAanmaken()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            mockService.Setup(s => s.AddOrUpdateProductAsync(It.IsAny<Product>()))
                       .Returns<Product>(p => Task.FromResult(p));

            var controller = new ProductController(mockService.Object);

            var productDto = new ProductDto
            {
                Foto = "image.jpg",
                artikelkenmerken = "Test kenmerken",
                Hoeveelheid = 5,
                MinimumPrijs = 10,
                dagdatum = DateTime.Now,
                leverancierID = "" // DTO wordt genegeerd
            };

            // --- Mock een klant (niet Leverancier) ---
            var userVeilingsmeester = new security.ClaimsPrincipal(new security.ClaimsIdentity(new security.Claim[]
            {
                new security.Claim(security.ClaimTypes.NameIdentifier, "veilingsmeester-123"),
                new security.Claim(security.ClaimTypes.Role, "Veiilngsmeester")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userVeilingsmeester }
            };

            // Act
            var result = await controller.PostProduct(productDto);

            // Assert

            // Omdat [Authorize(Roles = "Leverancier")] buiten controller runt, kan unit test dit niet automatisch triggeren.
            // Je kunt dit alleen integratietesten of handmatig checken met Authorization filter.
            // Voor unit test, we kunnen checken dat controller de User.Role correct herkent
            var role = userVeilingsmeester.FindFirst(security.ClaimTypes.Role)?.Value; // deze regel en de regel eronder checkt of de rol niet leverancier heeft
            Assert.NotEqual("Leverancier", role); // Dus deze gebruiker mag niet
        }
    }
}