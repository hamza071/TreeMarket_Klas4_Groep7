namespace TreeMarket_Klas4_Groep7.Views
{
    public class Veilingsmeester
    {
        public int LeverancierId { get; set; }
        public DateTime PlanDatum { get; set; }
        //Foreign key naar gebruiker
        public int GebruikerId { get; set; }
    }
}