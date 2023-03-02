namespace EsPark_WebApplication.Helper.DTO.CreateInvoice
{
    public class CreateInvoiceResponse
    {
        public List<PdfItem> PdfItems { get; set; }
    }
    public class PdfItem
    {
        public bool status { get; set; }
        public string Url { get; set; }
        public string Fno { get; set; }
        public string Messages { get; set; }
        public string XmlFileName { get; set; }
        public string Ettn { get; set; }
        public int LocalInvoiceId { get; set; }
    }

}
