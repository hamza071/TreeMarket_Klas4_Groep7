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
                //De dashboard bevat maar 1 product
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

            //kijken of een veilingsmeester binnen de dashboard kan komen om de
            controller.ControllerContext = TestAuthHelper.CreateContext("veilingsmeester-123", "Veilingsmeester");

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Dashboard>>(ok.Value);

            Assert.Equal("Appel", data.First().Product.ProductNaam);
        }


        // Welke rollen geen toegang tot de dashboard heeft
        [Fact]
        public async Task GetAnomous_ReturnsForbid_For_UnauthorizedRole()
        {
            var controller = new DashboardsController(Mock.Of<IDashboardService>());

            controller.ControllerContext = TestAuthHelper.CreateContext("user-123", "");

            var result = await controller.GetAll();

            Assert.IsType<ForbidResult>(result);
        }
    }
}