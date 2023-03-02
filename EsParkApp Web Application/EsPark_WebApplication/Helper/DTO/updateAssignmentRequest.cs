namespace EsPark_WebApplication.Helper.DTO
{
    public class updateAssignmentRequest
    {
        public int updateId { get; set; }
        public int lokasyonAdi { get; set; }
        public int cihazNo { get; set; }
        public int kullaniciAdi { get; set; }
        public int recby { get; set; }
        public int isactive { get; set; }
        public DateTime recdate { get; set; }
    }
}
