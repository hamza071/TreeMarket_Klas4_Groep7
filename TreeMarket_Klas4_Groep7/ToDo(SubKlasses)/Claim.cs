namespace TreeMarket_Klas4_Groep7.Views
{
    public class Claim
    {
        public int ClaimID {  get; set; }
        //Het staat in de RIM als INT, maar de rest is decimal
        public decimal Prijs { get; set; }
        //Foreign key
        public int KlantId { get; set; }
        public int VeilingId { get; set; }
    }
}
