namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class showParkingsRequest
    {
        public string DT_ParkingsId { get; set; }
        public int parkingId { get; set; }
        public int parkingDebtId { get; set; }
        public string licenseplate { get; set; }
        public string? startdate { get; set; }
        public string? enddate { get; set; }
        public int parkingduration { get; set; }
        public decimal parkingfee { get; set; }
        public decimal? parkingdurationforovertime { get; set; }
        public decimal? paidfeeforovertime { get; set; }
        public decimal? parkingfeeforovertime { get; set; }
        public string deviceId { get; set; }
        public string? locId { get; set; }
        public string? rowId { get; set; }

        public DateTime date { get; set; }
        public bool? lawyer { get; set; }
    }
}
