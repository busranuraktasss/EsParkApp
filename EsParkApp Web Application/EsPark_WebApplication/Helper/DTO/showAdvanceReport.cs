namespace EsPark_WebApplication.Helper.DTO
{
    public class showAdvanceReport
    {
        public string DT_ReportId { get; set; }
        public int reportId { get; set; }
        public string locname { get; set; } 
        public string username { get; set; }
        public string device { get; set; }
        public string? plaka { get; set; }
        public DateTime startdate { get; set; }
        public int parking_duration { get; set; }
        public decimal parking_fee { get; set; }    
        public int? parking_duration_forovertime { get; set; }
        public decimal? parking_fee_forovertime { get; set; }
        public decimal? paid_fee_forovertime { get; set; }   
        public DateTime? outdate { get; set; }
        public string? indebted { get; set; }
    }
}
