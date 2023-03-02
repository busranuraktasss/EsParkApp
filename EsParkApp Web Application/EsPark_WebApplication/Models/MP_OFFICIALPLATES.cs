using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_OFFICIALPLATES
    {
		[Key]
		public int PID;
		public string LICENSEPLATE;
		public int FREETIME;
		public string? GRUP;
		public DateTime FINISHDATE;
		public decimal? FEE;
		public bool IsCCard;
		public Guid? ETTN;


		[ForeignKey("MP_CUSTOMERS")]
		public int CUSTOMERBAG { get; set; }
		
		//[ForeignKey("MP_PARKINGS")]
		//public int PARKINGS { get; set; }

	}
}
