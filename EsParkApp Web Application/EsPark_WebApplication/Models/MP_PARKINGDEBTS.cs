using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_PARKINGDEBTS
    {
		[Key]
		public int PARKINGDEBTID;
		public string LICENSEPLATE;
		public decimal DEBTAMOUNT;
		public string? EXPLANATION;
		public DateTime? FINISHDATE;
		public decimal? FEE;
		public bool? LAWYER;


		[ForeignKey("MP_PARKINGS")]
		public int PARKINGID { get; set; }

		[ForeignKey("MP_DEBTPAYMENT")]
		public int? DEBTPAYMENTID { get; set; }	
		
		[ForeignKey("MP_JOBROTATIONHISTORY")]
		public int PARKINGJRID { get; set; }


	}
}
