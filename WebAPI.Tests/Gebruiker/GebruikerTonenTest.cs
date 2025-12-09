using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using TreeMarket_Klas4_Groep7.Controllers;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Xunit;

namespace WebAPI.Tests.TGebruiker
{
    public class GebruikerControllerTests
    {
        //Deze test maakt geen gebruik van de database, maar een Mock
        //In het kort ApiContext wordt gebruikt binnen
        //ApiContext > IGebruikerController (interface) > GebruikerService (maakt gebruik van ApiContext) > GebruikerController maakt gebruik van de service.
        [Fact]
        public async Task CreateUserKlant_ReturnsOk_WhenValid()
        {
            //==== Arrange====
            var mockRepo = new Mock<IGebruikerController>();

            // Simuleer dat de email nog niet bestaat
            mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Gebruiker)null);

            // Simuleer toevoegen (kan leeg zijn, want we testen alleen logica)
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Gebruiker>())).Returns(Task.CompletedTask);

            var controller = new GebruikerController(mockRepo.Object, new PasswordHasher<Gebruiker>(), null);

            var klantDto = new KlantDto
            {
                Naam = "Zygarde",
                Email = "zygarde@test.com",
                Wachtwoord = "!Test1234",
                Telefoonnummer = "0612345678"
            };

            // ====Act===
            var result = await controller.CreateUserKlant(klantDto);

            //==== Assert===
            var okResult = Assert.IsType<OkObjectResult>(result);
            var klant = Assert.IsType<Klant>(okResult.Value);
            Assert.Equal("Zygarde", klant.Naam);
        }

        [Fact]
        public async Task ShowAllUsers_ReturnsAllUsers()
        {
            // Arrange: maak een mock van de service/interface
            var mockService = new Mock<IGebruikerController>();

            // Simuleer wat data die de service zou teruggeven
            var fakeUsers = new List<Gebruiker>
            {
                new Gebruiker { GebruikerId = 1, Naam = "Socrates", Email = "socrates@test.com", Rol = "Klant" },
                new Gebruiker { GebruikerId = 2, Naam = "Herbert Marcuse", Email = "herbertmarcuse@test.com", Rol = "Leverancier" }
            };

            // Stel in dat GetAllAsync() de fakeUsers teruggeeft
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeUsers);

            // Injecteer de mock in de controller
            var controller = new GebruikerController(
                mockService.Object,
                new PasswordHasher<Gebruiker>(),
                null 
            );

            // Act: roep de endpoint aan
            var result = await controller.GetAllUsers();

            // Assert: controleer of het resultaat OkObjectResult is en de juiste data bevat
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<Gebruiker>>(okResult.Value);

            // 2 gebruikers worden verwacht
            Assert.Equal(2, users.Count()); 
            Assert.Contains(users, u => u.Naam == "Socrates");
            Assert.Contains(users, u => u.Naam == "Herbert Marcuse");
        }


    }
}
