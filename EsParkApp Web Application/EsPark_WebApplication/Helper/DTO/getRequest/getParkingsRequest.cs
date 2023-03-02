namespace EsPark_WebApplication.Helper.DTO.getRequest
{
    public class getParkingsRequest
    {
        public string DT_ParkingsId { get; set; }
        public int parkingId { get; set; }
        public string? licenseplate { get; set; }
        public DateTime startdate { get; set; }
        public int parkingduration { get; set; }
        public decimal parkingfee { get; set; }
        public int? parkingdurationforovertime { get; set; }
        public decimal? parkingfeeforovertime { get; set; }

        public decimal? paidfeeforovertime { get; set; }
        public decimal paidfee { get; set; }
        public DateTime enddate { get; set; }
        public DateTime? outdate { get; set; }
        public string? deviceId { get; set; }
        public string? locId { get; set; }
        public string? rowId { get; set; }
        public string indebted { get; set; }

    }
}
