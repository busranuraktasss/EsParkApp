using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_INVOICE_INFOS
    {
        [Key]
		public int Id;
		public int CreateUserId;//---
		public DateTime CreateDate;
		public string InvoiceText;
		public int InvoiceType;
	}
}
