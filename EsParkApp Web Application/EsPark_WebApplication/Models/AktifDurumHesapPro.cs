using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class AktifDurumHesapPros
    {
        [Key]
        public string ID { get; set; }
        public DateTime trh { get; set; }
        public string USERNAME { get; set; }
        public string LOCNAME { get; set; }
        public string SERIALNO { get; set; }
        public string CREATEDDATE { get; set; }
        public string ARAC { get; set; }
        public string TAHSIL { get; set; }
        public string TAHSIL_KART { get; set; }
        public string BORC { get; set; }
        public string BORC_KART { get; set; }
        public string TOPLAM { get; set; }
        public string TOPLANMASI_GEREKEN { get; set; }
        public string BATTERY { get; set; }
        public string DOLULUK { get; set; }


    }
}
