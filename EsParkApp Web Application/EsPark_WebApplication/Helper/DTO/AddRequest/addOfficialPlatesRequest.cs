namespace EsPark_WebApplication.Helper.DTO.AddRequest
{
    public class addOfficialPlatesRequest
    {
        public int deviceId { get; set; }
        public string licenseplate { get; set; }
        public int freetime { get; set; }
        public DateTime? finishdate { get; set; }
        public DateTime startdate { get; set; }
        public int month { get; set; }
        public int alınanTutar { get; set; }
        public decimal? fee { get; set; }
        public int cusId { get; set; }
        public string grup { get; set; }
        public int locId { get; set; }
        public int plateId { get; set; }
    }
}
