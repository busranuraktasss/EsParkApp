using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_LOCATIONS
    {
		[Key]
		public int LOCID;
		public string LOCNAME;
		public string LOCCODE;
		public string LOCADDRESS;
		public int CAPACITY;
		public int LOCTYPE;
		public int CENTERLOCID;//---
		public int ISACTIVE;
		public int ISDELETED;
		public int RECBY;
		public DateTime RECDATE;
		public string PHONE;
		public string MUHKOD;
	}
}
