namespace EsPark_WebApplication.Helper.DTO.UpdateRequest
{
    public class updateOfficialPlatesRequest
    {
        public int updateId2 { get; set; }
        public string licenseplate { get; set; }
        public int freetime { get; set; }
        public DateTime finishdate { get; set; }
        public decimal? fee { get; set; }
        public int locId { get; set; }
        public int cusId { get; set; }
        public string grup { get; set; }
        public Guid? ETTN { get; set; }

    }
}

