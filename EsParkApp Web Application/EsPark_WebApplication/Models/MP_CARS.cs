using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_CARS
    {
		[Key]
		public decimal ID;
		public decimal? CUSTOMERBAG;
		public string PLATE;
		public string MARKA;
		public string MODEL;
		public string TIP;
		public decimal? YIL;
		public string ACIKLAMA;
		public DateTime? BITISTARIHI;
		public decimal? UCRET;
	}
}
