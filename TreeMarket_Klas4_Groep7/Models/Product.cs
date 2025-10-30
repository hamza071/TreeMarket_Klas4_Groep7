namespace TreeMarket_Klas4_Groep7.Views
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Foto { get; set; }
        public string Artikelkenmerken { get; set; }
        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }
        public DateTime Dagdatum { get; set; }
        //Een op meer relatie
        public decimal LeverancierID { get; set; }
    }
}