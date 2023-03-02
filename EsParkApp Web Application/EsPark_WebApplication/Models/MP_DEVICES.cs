using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_DEVICES
    {
		[Key]
		public int DEVICEID;
		public string SERIALNO;
		public int SIMID;//---
		public string ICCID;//---
		public int ISACTIVE;
		public int ISDELETED;
		public int TERMTYPE;
		public int BATTERY_STATUS;
		public string DEVICETYPE;

	}
}
