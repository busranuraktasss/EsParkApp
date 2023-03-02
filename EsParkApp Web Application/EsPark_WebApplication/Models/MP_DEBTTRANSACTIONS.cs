using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_DEBTTRANSACTIONS
    {
        [Key]
		public int DEBTTRANSACTIONID;
		public DateTime RECEIVEDDATE;
		public DateTime CREATEDDATE;
		public int JOBROTATIONHISTORYID;
		public string DATA;

	}
}
