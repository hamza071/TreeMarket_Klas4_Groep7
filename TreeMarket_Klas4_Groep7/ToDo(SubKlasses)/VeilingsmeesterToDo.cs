namespace TreeMarket_Klas4_Groep7.ToDo_SubKlasses_
{
    public class VeilingsmeesterToDo
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public DateTime PlanDatum { get; set; } = DateTime.Now;
        public string Wachtwoord { get; set; }
    }
}
