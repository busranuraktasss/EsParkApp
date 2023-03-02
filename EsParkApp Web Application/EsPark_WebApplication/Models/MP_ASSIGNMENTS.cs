using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_ASSIGNMENTS
    {
		[Key]
		public int ASSIGNMENTID;
		public int ISDELETED;
		public int ISACTIVE;
		public int RECBY;

        public DateTime RECDATE { get; set; }


		[ForeignKey("MP_LOCATIONS")]
		public int LOCID { get; set; }

		[ForeignKey("MP_USERS")]
		public int USERID { get; set; }

		[ForeignKey("MP_DEVICES")]
		public int TERMID { get; set; }

	}
}


