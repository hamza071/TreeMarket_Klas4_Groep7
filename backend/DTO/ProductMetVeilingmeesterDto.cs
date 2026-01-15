namespace backend.DTO
{
   
    public class ProductMetVeilingmeesterDto
    {
        public int ProductId { get; set; }
        public string? Naam { get; set; }            
        public string? Omschrijving { get; set; }   
        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }
        public string? Foto { get; set; }           
        public string? Status { get; set; }
        public string? LeverancierNaam { get; set; } 
    }
}