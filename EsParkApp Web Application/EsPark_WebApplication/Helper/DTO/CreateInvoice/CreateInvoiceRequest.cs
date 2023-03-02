namespace EsPark_WebApplication.Helper.DTO.CreateInvoice
{
    public class CreateInvoiceRequest
    {
        public List<InvoiceInfo> InvoiceInfo { get; set; }
    }
    public class InvoiceInfo
    {
        public int tutar { get; set; }
        public string Plaka { get; set; }
        public string FisYazisi { get; set; }
        public string MuhKodu { get; set; }
        public List<int> KayitNo { get; set; }
        public int JobId { get; set; }
        public int PayType { get; set; }
        public string Ettn { get; set; }
        public string EInvoiceNumber { get; set; }
        public DateTime NowTime { get; set; }
    }
}