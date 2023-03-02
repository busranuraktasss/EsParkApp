using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_NOTIFICATIONS
    {
		[Key]
		public int ID;
		public DateTime? NF_DATE;
		public string NF_MESSAGE;
		public int? JOBID;
		public bool? ISREADED;
		public bool? ISALLSEND;
		public string TITLE;
		public int? SENDUSERID;//---

	}
}
