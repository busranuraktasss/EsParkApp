using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_LOCTARIFFASSIGNMENTS
    {
        [Key]
		public int ASSIGNID;
		public int ISACTIVE;
		public int ISDELETED;
		public int RECBY;
		public DateTime RECDATE;


		[ForeignKey("MP_LOCATIONS")]
		public int LOCID { get; set; }	

		[ForeignKey("MP_PARKTARIFFS")]
		public int TARIFFID { get; set; }	
		
		[ForeignKey("MP_CAMPAIGNS")]
		public int CAMPAIGNID { get; set; }	
	}
}
