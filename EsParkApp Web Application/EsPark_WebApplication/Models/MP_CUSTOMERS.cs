using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_CUSTOMERS
    {
		[Key]
		public int Id;
		public string Adisoyadi;
		public string Tel1;
		public string Tel2;
		public string Mail;
		public decimal Tckimlik;
		public string Adres;
		public string Meslek;
		public string Ogretim;
		public string İsyeri;
		public string Gorevi;
		public string Medeni;
		public short? Tipi;
		public DateTime? Dtarihi;
		public string Vergidairesi;
	}
}
