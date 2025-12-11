namespace backend.Models.DTO
{
    // ✅ DTO voor Product + gekoppelde Leverancier
    // Ideaal voor lijstweergave of overzicht waarbij leverancier info nodig is
    public class ProductMetLeverancierDto
    {
        public int ProductId { get; set; }         // Product ID
        public decimal MinimumPrijs { get; set; }  // Minimumprijs
        public string LeverancierNaam { get; set; } // Naam van de leverancier
    }
}