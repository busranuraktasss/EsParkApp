namespace EsPark_WebApplication.Helper.DTO.UpdateRequest
{
    public class updateLocationRequest
    {
        public int updateId { get; set; }
        public string locname { get; set; }
        public string locaddress { get; set; }
        public int capacity { get; set; }
        public string phone { get; set; }
        public string muhkod { get; set; }
        public DateTime recdate { get; set; }
        public int isactive { get; set; }
    }
}
