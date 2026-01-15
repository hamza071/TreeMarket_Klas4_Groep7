using backend.Controllers;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Tests.Helpers;
using security = System.Security.Claims;

namespace WebAPI.Tests.TDashboard
{
    public class DashboardTestRead
    {

        // Dashboard wat er getoont wordt (product)
        [Fact]
        public async Task GetAll_Returns_Dashboard_With_Product()
        {
            // Arrange
            var dashboards = new List<Dashboard>
            {
                new Dashboard
                {
                    DashboardID = 1,
                    Product = new Product
                    {
                        ProductId = 1,
                        ProductNaam = "Appel"
                    },
                }
            };

            var mockService = new Mock<IDashboardService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(dashboards);

            var controller = new DashboardsController(mockService.Object);

            controller.ControllerContext = TestAuthHelper.CreateContext("admin-123", "Admin");

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Dashboard>>(ok.Value);

            Assert.Equal("Appel", data.First().Product.ProductNaam);
        }


        // Welke rollen geen toegang tot de dashboard heeft
        [Fact]
        public async Task GetKlant_ReturnsForbid_For_UnauthorizedRole()
        {
            var controller = new DashboardsController(Mock.Of<IDashboardService>());

            controller.ControllerContext = TestAuthHelper.CreateContext("klant-123", "Klant");

            var result = await controller.GetAll();

            Assert.IsType<ForbidResult>(result);
        }
    }
}