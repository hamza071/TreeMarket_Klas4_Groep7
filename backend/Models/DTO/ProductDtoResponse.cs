namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class ProductDtoResponse
    {
        public int ProductId { get; set; }
        public string Naam { get; set; }
        public string Variëteit { get; set; }
        public string Kleur { get; set; }
        public int Aantal { get; set; }
        public string Omschrijving { get; set; }
        public decimal MinimumPrijs { get; set; }
        public string AfbeeldingUrl { get; set; }
    }
}