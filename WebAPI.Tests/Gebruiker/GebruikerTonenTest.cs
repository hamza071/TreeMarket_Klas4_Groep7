using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Moq;
using TreeMarket_Klas4_Groep7.Controllers;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using Xunit;

namespace WebAPI.Tests
{
    public class GebruikerControllerTests
    {
        [Fact]
        //De testfunctie is async, omdat de GetAllUser funcie in de 
        public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange: In-memory database optuigen
            var options = new DbContextOptionsBuilder<ApiContext>()
                //Maakt een test database gemaakt om te testen of het werkt.
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new ApiContext(options);

            //Vult de data van de gebruiker in om te testen of het werkt
            context.Gebruiker.Add(new Gebruiker { 
                Naam = "Test", 
                Email = "test@test.com",
                Rol = "klant",
                Wachtwoord = "!Test1234"
            });
            context.SaveChanges();

            var hasher = new PasswordHasher<Gebruiker>();
            var mockConfig = new Mock<IConfiguration>();

            var controller = new GebruikerController(context, hasher, mockConfig.Object);

            // Act: Gaat testen of het wel echt werkt of niet
            var result = await controller.GetAllUsers();

            // Assert: controleert of de test werkt of niet
            var ok = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<Gebruiker>>(ok.Value);
            Assert.Single(users);
        }


        //[Fact]
        ////De testfunctie is async, omdat de GetAllUser funcie in de 
        //public async Task GetID_User_Test()
        //{
        //    // Arrange: In-memory database optuigen
        //    var options = new DbContextOptionsBuilder<ApiContext>()
        //        //Maakt een test database gemaakt om te testen of het werkt.
        //        .UseInMemoryDatabase(databaseName: "TestDb")
        //        .Options;

        //    var context = new ApiContext(options);

        //    //Vult de data van de gebruiker in om te testen of het werkt
        //    context.Gebruiker.Add(new Gebruiker
        //    {
        //        GebruikerId = 1,
        //        Naam = "Test2",
        //        Email = "test2@test.com",
        //        Rol = "klant",
        //        Wachtwoord = "!Test1234"
        //    });
        //    context.SaveChanges();

        //    var hasher = new PasswordHasher<Gebruiker>();
        //    var mockConfig = new Mock<IConfiguration>();

        //    var controller = new GebruikerController(context, hasher, mockConfig.Object);

        //    // Act: Gaat testen of het wel echt werkt of niet
        //    var result = await controller.GetUserById();

        //    // Assert: controleert of de test werkt of niet
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    var users = Assert.IsAssignableFrom<IEnumerable<Gebruiker>>(ok.Value);
        //    Assert.Single(users);
        //}
    }
}
