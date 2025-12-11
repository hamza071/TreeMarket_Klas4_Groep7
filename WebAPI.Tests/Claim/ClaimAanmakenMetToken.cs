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
using backend.Services;
using security = System.Security.Claims;


namespace WebAPI.Tests.TClaim
{
    public class ClaimAanmakenMetToken
    {
        // ==========Claim kan alleen aangemaakt worden wanneer het een token heeft ============
        [Fact]
        public async Task PostClaim_Succesvol_MetToken()
        {
            // Arrange
            var mockService = new Mock<IClaimController>();
            mockService.Setup(s => s.CreateClaimAsync(It.IsAny<ClaimDto>(), "user-123"))
                       .ReturnsAsync((ClaimDto dto, string userId) => new Claim
                       {
                           ClaimID = 1,
                           VeilingId = dto.veilingId,
                           Prijs = dto.prijs,
                           KlantId = userId
                       });

            var controller = new ClaimController(mockService.Object);

            // --- Mock een ingelogde gebruiker ---
            var user = new security.ClaimsPrincipal(new security.ClaimsIdentity(new security.Claim[]
            {
                new security.Claim(security.ClaimTypes.NameIdentifier, "user-123")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var claimDto = new ClaimDto { veilingId = 5, prijs = 100 };

            // Act
            var result = await controller.PostClaim(claimDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var claim = Assert.IsType<Claim>(okResult.Value);

            Assert.Equal("user-123", claim.KlantId);
            Assert.Equal(5, claim.VeilingId);
            Assert.Equal(100, claim.Prijs);
        }

        // ========== Het zou bij niet-ingelogde gebruiker moeten falen ==========
        [Fact]
        public async Task PostClaim_Faalt_ZonderToken()
        {
            var mockService = new Mock<IClaimController>();
            var controller = new ClaimController(mockService.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new security.ClaimsPrincipal() }
            };

            var claimDto = new ClaimDto { veilingId = 5, prijs = 100 };

            var result = await controller.PostClaim(claimDto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Je moet ingelogd zijn om te bieden.", unauthorized.Value);
        }
    }
}