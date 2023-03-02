using Microsoft.VisualBasic;

namespace EsPark_WebApplication.Helper.DTO.AddRequest
{
    public class addAssignmentRequest
    {
        public int assignId { get; set; }
        public int lokasyonAdi { get; set; }
        public int cihazNo { get; set; }
        public int kullaniciAdi { get; set; }
        public int recby { get; set; }
        public int isactive { get; set; }

        public DateTime recdate { get; set; }
    }
}
