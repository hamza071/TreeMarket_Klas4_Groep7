namespace backend.DTO
{
    public class HistoryDto
    {
        public string Datum { get; set; }
        public string Aanbieder { get; set; }
        public decimal Prijs { get; set; }
    }
    
    public class ProductHistoryResponse
    {
        public List<HistoryDto> EigenHistorie { get; set; } // Van deze aanbieder
        public List<HistoryDto> MarktHistorie { get; set; } // Van iedereen
        public decimal GemiddeldeEigen { get; set; }
        public decimal GemiddeldeMarkt { get; set; }
    }
}