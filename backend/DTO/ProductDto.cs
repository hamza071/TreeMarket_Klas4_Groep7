namespace backend.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Foto { get; set; }

        public string Naam { get; set; }         
        public string Omschrijving { get; set; }   

        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }

        public DateTime Dagdatum { get; set; } = DateTime.Now;

        public string LeverancierID { get; set; }
    }
}