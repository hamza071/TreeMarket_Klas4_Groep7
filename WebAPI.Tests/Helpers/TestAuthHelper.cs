using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using security = System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

//Deze klasse wordt gebruikt omdat we gebruik maken van de token, moet je dit gebruiken,
//anders herkend ons programma niet dat de gberuiker ingelogd is. 

namespace WebAPI.Tests.Helpers
{
    public class TestAuthHelper
    {
        //De functie zorgt ervoor dat je gebruikers kunt gaan testen.
        //Deze functie voorkomt ook DRY (Don't Repeat Yourself)
        public static ControllerContext CreateContext(string userId, string role)
        {
            //Twee rollen zodat de test zowel de id en rol kunt gebruiken voor een gebruiker
            var user = new security.ClaimsPrincipal(new security.ClaimsIdentity(new[]
            {
                new security.Claim(security.ClaimTypes.NameIdentifier, userId),
                new security.Claim(security.ClaimTypes.Role, role)
            }, "mock"));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        //Hier wordt mock-gebruikers gemaakt om te testen.
        public static UserManager<Gebruiker> MockUserManager(string role)
        {
            var store = new Mock<IUserStore<Gebruiker>>();
            var manager = new Mock<UserManager<Gebruiker>>(
                store.Object, null, null, null, null, null, null, null, null);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync(new Gebruiker());

            manager.Setup(x => x.GetRolesAsync(It.IsAny<Gebruiker>()))
                   .ReturnsAsync(new List<string> { role });

            return manager.Object;
        }
    }
}
