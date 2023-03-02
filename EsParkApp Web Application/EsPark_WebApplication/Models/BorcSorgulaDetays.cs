using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class BorcSorgulaDetays
    {
        //--
        public int Id { get; set; }
        public string Plaka { get; set; }
        public string Borc { get; set; }
        public string Alinan { get; set; }
        public string PARKINGDEBTID { get; set; }
        public string STARTDATE { get; set; }
        public string OUTDATE { get; set; }
        public string BORC { get; set; }
        public string LOCNAME { get; set; }
        public string REALNAME { get; set; }
    }
}
