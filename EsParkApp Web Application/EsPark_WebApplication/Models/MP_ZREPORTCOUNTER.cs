using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_ZREPORTCOUNTER
    {
        [Key]
		public int ID;
		public int LASTCOUNT;
		public DateTime LASTUPDATETIME;


		[ForeignKey("MP_DEVICES")]
		public int DEVICEID { get; set; }

	}
}
