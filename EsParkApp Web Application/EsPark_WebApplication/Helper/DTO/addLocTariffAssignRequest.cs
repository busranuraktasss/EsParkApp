namespace EsPark_WebApplication.Helper.DTO
{
    public class addLocTariffAssignRequest
    {
        public int assignId { get; set; }
        public int isactive { get; set; }
        public int recby { get; set; }
        public DateTime recdate { get; set; }
        public int locId { get; set; }
        public int TariffId { get; set; }
       
	}
}
