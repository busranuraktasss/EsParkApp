using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
	public class MP_PARKINGS
	{
		[Key]
		public int PARKINGID { get; set; }
		public string? SESSIONID { get; set; }//---
		public int PARKINGSTATUS { get; set; }
		public string LICENSEPLATE { get; set; }
		public DateTime STARTDATE { get; set; }
		public DateTime ENDDATE { get; set; }
		public int PERON { get; set; }
		public int PARKINGDURATION { get; set; }
		public decimal PARKINGFEE { get; set; }
		public decimal PAIDFEE { get; set; }
		public int? PARKINGDURATIONFOROVERTIME { get; set; }
		public decimal? PARKINGFEEFOROVERTIME { get; set; }
		public decimal? PAIDFEEFOROVERTIME { get; set; }
		public DateTime? ENDDATEFOROVERTIME { get; set; }
		public DateTime? OUTDATE { get; set; }
		public int? INDEBTED { get; set; }
		public decimal? GBORC { get; set; }
		public int? JOB_OUT { get; set; }
		public bool CCart { get; set; }
		public int? LocaliD { get; set; }
		public string? ImageName { get; set; }
		public int? OLD_ID { get; set; }
		public bool CCartEntry { get; set; }
		public int? LocLink { get; set; }
		public Guid? IN_ETTN { get; set; }
		public Guid? OUT_ETTN { get; set; }
		public string? IN_F_NO { get; set; }
		public string? OUT_F_NO { get; set; }
		public string? VerifyEnrollment_Out { get; set; }
		public string? VerifyEnrollment_In { get; set; }

		[ForeignKey("MP_USERS")]
		public int ROWID { get; set; }

		[ForeignKey("MP_PARKTARIFFS")]
		public int TARIFFID { get; set; }

		[ForeignKey("MP_JOBROTATIONHISTORY")]
		public int JOBROTATIONHISTORYID { get; set; }

	}
}
