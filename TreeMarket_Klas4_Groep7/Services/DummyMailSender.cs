using Microsoft.AspNetCore.Identity;
using TreeMarket_Klas4_Groep7.Models;

namespace TreeMarket_Klas4_Groep7.Services
{
    public class DummyEmailSender : IEmailSender<Gebruiker>
    {
        public Task SendConfirmationLinkAsync(Gebruiker user, string email, string confirmationLink)
        {
            Console.WriteLine($"Bevestiging sturen naar {email}: {confirmationLink}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(Gebruiker user, string email, string resetLink)
        {
            Console.WriteLine($"Wachtwoord reset link voor {email}: {resetLink}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(Gebruiker user, string email, string resetCode)
        {
            Console.WriteLine($"Wachtwoord reset code voor {email}: {resetCode}");
            return Task.CompletedTask;
        }
    }
}