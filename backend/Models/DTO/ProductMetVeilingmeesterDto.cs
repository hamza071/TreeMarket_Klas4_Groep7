public class ProductMetVeilingmeesterDto
{
    public int ProductId { get; set; }
    public string? Naam { get; set; }            // ProductNaam
    public string? Varieteit { get; set; }       // Varieteit
    public string? Omschrijving { get; set; }   // Omschrijving
    public int Hoeveelheid { get; set; }
    public decimal MinimumPrijs { get; set; }
    public string? Foto { get; set; }           // Foto
    public string? Status { get; set; }
    public string? LeverancierNaam { get; set; }
}
