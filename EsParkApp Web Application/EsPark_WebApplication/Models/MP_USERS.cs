using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_USERS
    {
		[Key]
		public int ROWID { get; set; }
		public string USERNAME { get; set; }
		public string PASSWORD { get; set; }
		public string REALNAME { get; set; }
		public int ISDELETED { get; set; }
		public int STATUS { get; set; }
		public int AUTHORITY { get; set; }
		public string PHONE { get; set; }

	}
}
