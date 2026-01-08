using backend.Controllers;
using backend.DTO;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using security = System.Security.Claims;


namespace WebAPI.Tests.TProduct
{
    public class ProductValidatievelden
    {
        // ============LeverancierID mag niet NULL zijn =============
        [Fact]
        public async Task ProductKanNietAangemaaktWorden_ZonderToken()
        {
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal() // 👈 geen identity
                }
            };

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            var result = await controller.CreateProduct(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Je bent niet ingelogd.", unauthorized.Value);
        }


        //========Klant mag geen product aanmaken======
        [Fact]
        public async Task KlantKanProductNietAanmaken()
        {
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            var user = new security.ClaimsPrincipal(
                new security.ClaimsIdentity(new[]
                {
            new security.Claim(security.ClaimTypes.NameIdentifier, "klant-123"),
            new security.Claim(security.ClaimTypes.Role, "Klant")
                }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            var result = await controller.CreateProduct(dto);

            Assert.IsType<ForbidResult>(result);
        }


        //===== Veilingsmeester en Admin mogen ook geen producten aanmaken ===============
        [Fact]
        public async Task VeilingsmeesterKanProductNietAanmaken()
        {
            var mockService = new Mock<IProductService>();
            var controller = new ProductController(mockService.Object);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
            new security.Claim(ClaimTypes.NameIdentifier, "veilingsmeester-123"),
            new security.Claim(ClaimTypes.Role, "Veilingsmeester")
                }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var dto = new ProductUploadDto
            {
                ProductNaam = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 10
            };

            var result = await controller.CreateProduct(dto);

            Assert.IsType<ForbidResult>(result);
        }

    }
}