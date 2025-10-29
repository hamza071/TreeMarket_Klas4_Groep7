namespace TreeMarket_Klas4_Groep7.Views
{
    public class Leverancier : Gebruiker
    {
        public int LeverancierId {get; set;}
        public string bedrijf { get; set; }
        public string KvKNummer { get; set; }
        public string IBANnummer { get; set; }
        //Foreign key naar gebruiker
        public int GebruikerId { get; set; }
    }
}
