using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_JOBROTATIONHISTORY
    {
		[Key]
		public int ID;
		public DateTime CREATEDDATE;
		public DateTime BEGINDATE;
		public DateTime FINISHDATE;
		public int? OLD_ID;
		public string LOCATION { get; set; }


		[ForeignKey("MP_LOCATIONS")]
		public int PARKID { get; set; }

		[ForeignKey("MP_USERS")]
		public int USERID { get; set; }

        [ForeignKey("MP_DEVICES")]
        public int DEVICEID { get; set; }

  

    }

}

