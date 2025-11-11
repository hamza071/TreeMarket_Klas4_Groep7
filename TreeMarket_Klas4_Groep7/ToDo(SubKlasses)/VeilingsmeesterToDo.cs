namespace TreeMarket_Klas4_Groep7.ToDo
{
    public class VeilingsmeesterToDo
    {
        public int VeilingsmeesterId { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string? Telefoonnummer { get; set; }
        public DateTime PlanDatum { get; set; } = DateTime.Now;
        public string Wachtwoord { get; set; }
    }
}
