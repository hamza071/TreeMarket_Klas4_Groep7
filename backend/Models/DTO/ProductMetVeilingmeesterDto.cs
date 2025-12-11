namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    // DTO voor de veilingsmeester: product + leverancier info
    public class ProductMetVeilingmeesterDto
    {
        public int ProductId { get; set; }            // ProductId → frontend key
        public string? Name { get; set; }             // Artikelkenmerken
        public string? Description { get; set; }      // Optioneel
        public int Lots { get; set; }                 // Hoeveelheid
        public string? Image { get; set; }            // Foto
        public decimal MinimumPrijs { get; set; }     // MinimumPrijs
        public string? Status { get; set; }           // bv "pending"
        public string? LeverancierNaam { get; set; }  // Naam leverancier
    }
}