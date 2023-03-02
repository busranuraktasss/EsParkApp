namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class showOfficialPlatesRequest
    {
        public string DT_OffId { get; set; }
        public int pId { get; set; }
        public int cusId { get; set; }
        public string licenseplate { get; set; }
        public int freetime { get; set; }
        public string grup { get; set; }
        public string? finishdate { get; set; } 
        public decimal? fee { get; set; }
    }
}
