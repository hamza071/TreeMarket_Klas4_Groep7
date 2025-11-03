using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.ToDo_SubKlasses_
{
    public class LeverancierToDo
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public string bedrijf { get; set; }
        public string KvKNummer { get; set; }
        public string IBANnummer { get; set; }
        public string Wachtwoord { get; set; }
    }
}
