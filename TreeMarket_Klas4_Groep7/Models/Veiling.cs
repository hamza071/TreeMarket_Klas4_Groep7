namespace TreeMarket_Klas4_Groep7.Views
{
    public class Veiling
    {
        public int VeilingID { get; set; }
        //Status is met BIT. IK weet niet welke. Misschien int of bool
        public bool? Status { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal EindPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public int PrijsStap { get; set; }
        public int PrijsStrategie { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }

        //Is wel een foreignkey
        public int ProductID { get; set; }
        public int VeilingsmeesterID { get; set; }
    }
}