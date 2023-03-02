namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class showInvoicesRequest
    {
        public string DT_InvoiceId { get; set; }
        public long invoiceId { get; set; }    
        public string invoiceNumber { get; set; }   
        public string invoiceTime { get; set; }
        public decimal totalAmount { get; set; }    
        public decimal totalVat { get; set; }   
        public decimal totalSum { get; set; }   
        public string licensePlate { get; set; }
        public string invoiceInfo { get; set; }
        public Guid ettn { get; set; }
        public bool? checkCancel { get; set; }
        public DateTime? cancelTime { get; set; }
        public bool? checkCcart { get; set; }
        public string locId{ get; set; }
        public string rowId{ get; set; }
        public string deviceId{ get; set; }

    }
}
