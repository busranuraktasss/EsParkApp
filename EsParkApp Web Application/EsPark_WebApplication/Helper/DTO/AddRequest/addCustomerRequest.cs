namespace EsPark_WebApplication.Helper.DTO.AddRequest
{
    public class addCustomerRequest
    {
        public int cusId { get; set; }
        public string adsoyad { get; set; }
        public string tel1 { get; set; }
        public string tel2 { get; set; }
        public string mail { get; set; }
        public decimal tckimlik { get; set; }
        public string vergidairesi { get; set; }
        public DateTime tarih { get; set; }
        public string adres { get; set; }
        public string meslek { get; set; }
        public string ogretim { get; set; }
        public string isyeri { get; set; }
        public string gorevi { get; set; }
        public string medenihali { get; set; }
        public short tip { get; set; }
    }
}
