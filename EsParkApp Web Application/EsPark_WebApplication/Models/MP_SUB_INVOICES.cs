using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_SUB_INVOICES
    {
		[Key]
		public long Id;
		public long RecordId;//---

		[ForeignKey("MP_INVOICES")]
		public int InvoiceId { get; set; }

	}
}
