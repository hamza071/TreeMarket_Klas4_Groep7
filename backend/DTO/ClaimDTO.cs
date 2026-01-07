using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class ClaimDto
    {
        [Required]
        public decimal Prijs { get; set; }  // De prijs per stuk (of totaal, afhankelijk van je logica)

        [Required]
        public int VeilingId { get; set; }  // Welke veiling is het?

        [Required]
        public int Aantal { get; set; }     // <--- NIEUW: Hoeveel stuks wil de klant?
    }
}