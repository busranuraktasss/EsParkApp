using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_MAPLOCATIONS
    {
		[Key]
		public long Id;
		public string Lat;
		public string Lng;
		public int JobId;
		public int? BatteryStatus;
		public DateTime CreatedTime;

	}
}
