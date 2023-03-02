using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_TRANSACTIONS
    {
        [Key]
		public long TRANSACTIONID;
		public DateTime? RECEIVEDDATE;
		public DateTime? CREATEDDATE;
		public int? JOBROTATIONHISTORYID;
		public int? TRANSACTIONSTATUS;
		public int? TRANSACTIONTYPE;
		public string DATA;

	}
}
