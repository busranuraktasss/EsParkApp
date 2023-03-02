using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_DEBTPAYMENTS
    {
		[Key]
		public int DEBTPAYMENTID;
		public decimal DEBTPAYMENTAMOUNT;
		public int COLLECTIONJRID;//--
		public DateTime DEBTPAYMENTDATE;
		public string EXPLANATION;
		public bool? CCART;
		public Guid? ETTN;

		[ForeignKey("MP_COLLECTIONTYPES")]
		public int COLLECTIONTYPE { get; set; }

	}
}
