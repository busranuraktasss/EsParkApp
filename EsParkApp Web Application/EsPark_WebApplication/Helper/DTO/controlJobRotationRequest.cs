namespace EsPark_WebApplication.Helper.DTO
{
    public class controlJobRotationRequest
    {
        public int userId { get; set; } 
        public DateTime date { get; set; }  
        public int jobId { get; set; }  
        public string plaka { get; set; }
        public int plakaId { get; set; }
        public decimal ucret { get; set; }  

    }
}
