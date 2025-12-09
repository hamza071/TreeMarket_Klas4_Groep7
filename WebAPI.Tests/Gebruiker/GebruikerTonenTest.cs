using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using TreeMarket_Klas4_Groep7.Controllers;
using TreeMarket_Klas4_Groep7.Controllers.Interfaces;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;
using Xunit;

namespace WebAPI.Tests
{
    public class GebruikerControllerTests
    {
        //[Fact]
        //public async Task CreateUserKlant_ReturnsOk_WhenValid()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IGebruikerRepository>();

        //    // Simuleer dat de email nog niet bestaat
        //    mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Gebruiker)null);

        //    // Simuleer toevoegen (kan leeg zijn, want we testen alleen logica)
        //    mockRepo.Setup(r => r.AddAsync(It.IsAny<Gebruiker>())).Returns(Task.CompletedTask);

        //    var controller = new GebruikerController(mockRepo.Object, new PasswordHasher<Gebruiker>(), null);

        //    var klantDto = new KlantDto
        //    {
        //        Naam = "Danny",
        //        Email = "danny@test.com",
        //        Wachtwoord = "Test1234",
        //        Telefoonnummer = "0612345678"
        //    };

        //    // Act
        //    var result = await controller.CreateUserKlant(klantDto);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var klant = Assert.IsType<Klant>(okResult.Value);
        //    Assert.Equal("Danny", klant.Naam);
        //}

        //[Fact]
        ////De testfunctie is async, omdat de GetAllUser funcie in de 
        //public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        //{
        //    // Arrange: In-memory database optuigen. In het kort legers ophalen.
        //    var options = new DbContextOptionsBuilder<ApiContext>()
        //        //Maakt een test database gemaakt om te testen of het werkt.
        //        .UseInMemoryDatabase(databaseName: "TestDb")
        //        .Options;

        //    var context = new ApiContext(options);

        //    //Vult de data van de gebruiker in om te testen of het werkt
        //    context.Gebruiker.Add(new Gebruiker { 
        //        Naam = "Test", 
        //        Email = "test@test.com",
        //        Rol = "klant",
        //        Wachtwoord = "!Test1234"
        //    });
        //    context.SaveChanges();

        //    var hasher = new PasswordHasher<Gebruiker>();
        //    var mockConfig = new Mock<IConfiguration>();

        //    var controller = new GebruikerController(context, hasher, mockConfig.Object);

        //    // Act: Gaat testen of het wel echt werkt of niet
        //    var result = await controller.GetAllUsers();

        //    // Assert: controleert of de test werkt of niet
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    var users = Assert.IsAssignableFrom<IEnumerable<Gebruiker>>(ok.Value);
        //    Assert.Single(users);
        //}
    }
}
