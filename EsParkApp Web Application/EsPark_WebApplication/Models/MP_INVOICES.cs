using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_INVOICES
    {
		[Key]
		public long Id;
		public string InvoiceNumber;
		public decimal TotalAmount;
		public decimal TotalVat;
		public DateTime InvoiceTime;
		public int JobId;
		public string LicensePlate;
		public string InvoiceInfo;
		public Guid Ettn;
		public string AmountText;
		public long RecordId;
		public bool? Cancel;
		public DateTime? CancelTime;
		public bool? Ccart;


	}
}
