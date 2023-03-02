namespace EsPark_WebApplication.Helper.DTO
{
    public class getActiveShiftRequest
    {
        public int shift_no { get; set; }
        public string? loc_name { get; set; } 
        public string? user_name { get; set; }
        public string? device_no { get; set; }
        public decimal startdate_count { get; set; }
        public decimal outdate_count { get; set; }
        public decimal paid_fee { get; set; }
        

    }
}
