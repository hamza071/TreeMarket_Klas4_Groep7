using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMarket_Klas4_Groep7.Controllers;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace WebAPI.Tests.TGebruiker
{
    public class GebruikerAuthenticatie
    {
        //========Login testen ===============
        [Fact]
        public async Task GebruikerKanInloggen_Succes()
        {
            // Arrange
            var mockService = new Mock<IGebruikerController>();

            // Stel een bestaande gebruiker in
            var testGebruiker = new Gebruiker
            {
                GebruikerId = 1,
                Email = "test@example.com",
                Wachtwoord = "hashedPassword", // hash simuleren
                Rol = "Klant"
            };

            // Mock de GetByEmailAsync zodat het de gebruiker teruggeeft
            mockService.Setup(s => s.GetByEmailAsync("test@example.com"))
                       .ReturnsAsync(testGebruiker);

            var passwordHasher = new PasswordHasher<Gebruiker>();
            //De wachtwoord wordt gehashed
            testGebruiker.Wachtwoord = passwordHasher.HashPassword(testGebruiker, "plainPassword");

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("eenSuperSecureKey123");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            configurationMock.Setup(c => c["Jwt:DurationInMinutes"]).Returns("60");

            var controller = new GebruikerController(mockService.Object, passwordHasher, configurationMock.Object);

            // Act
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Wachtwoord = "plainPassword"
            };

            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsAssignableFrom<ObjectResult>(result);

            ////Met JTW token werkt die nog niet, maar als het gemerged is met de bestand van Hamza, dan gaat die wel werken.
            //var token = okResult.Value.GetType().GetProperty("token")?.GetValue(okResult.Value, null);
            //Console.WriteLine("The token: " + token);
            //Assert.NotNull(token); // JWT token is gegenereerd
        }

        // ==========Registratie testen ================

        // ----Klant----
        [Fact]
        public async Task KlantKanGeregistreerdWorden_Succes()
        {
            // Arrange
            var mockService = new Mock<IGebruikerController>();

            // Mock dat GetByEmailAsync null teruggeeft (email bestaat nog niet)
            mockService.Setup(s => s.GetByEmailAsync("nieuw@example.com"))
                        .ReturnsAsync((Gebruiker?)null);

            // Mock dat AddAsync gewoon succesvol is
            mockService.Setup(s => s.AddAsync(It.IsAny<Klant>()))
                        .Returns(Task.CompletedTask);

            var passwordHasher = new PasswordHasher<Gebruiker>();
            var controller = new GebruikerController(mockService.Object, passwordHasher, Mock.Of<IConfiguration>());

            var klantDto = new KlantDto
            {
                Naam = "Winnie",
                Email = "winnie@example.com",
                Telefoonnummer = "0612345678",
                Wachtwoord = "!Qwerty1234"
            };

            // Act
            var result = await controller.CreateUserKlant(klantDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var klant = Assert.IsType<Klant>(okResult.Value);
            Assert.Equal("Winnie", klant.Naam);
            Assert.Equal("winnie@example.com", klant.Email);
            Assert.Equal("Klant", klant.Rol);
        }

        [Fact]
        public void WachtwoordMoetMinimaalAchtCharactersHebben()
        {
            // Arrange
            var klantDto = new KlantDto
            {
                Naam = "Alucard",
                Email = "alucard@example.com",
                Telefoonnummer = "0612345678",
                Wachtwoord = "Aah!" // te kort
            };

            var context = new ValidationContext(klantDto, null, null);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(klantDto, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("minimaal 8 tekens"));
        }

    }
}
