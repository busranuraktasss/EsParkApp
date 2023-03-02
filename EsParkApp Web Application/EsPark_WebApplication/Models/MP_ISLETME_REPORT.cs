namespace EsPark_WebApplication.Models
{
    public class MP_ISLETME_REPORT
    {
        public long Id { get; set; }
        public decimal? Toplanmasi_gereken { get; set; }
        public decimal? Toplanan { get; set; }
        public decimal? Toplanan_nakit { get; set; }
        public decimal? Toplanan_kkarti { get; set; }
        public decimal? Borc { get; set; }
        public decimal? Borc_nakit { get; set; }
        public decimal? Borc_kkarti { get; set; }
        public decimal? Genel_nakit { get; set; }
        public decimal? Genel_kkart { get; set; }
        public decimal? Toplanan_genel { get; set; }
        public string Firma { get; set; }
        public DateTime? Tarih { get; set; }
        public string Locname { get; set; }
        public string Username { get; set; }
    }
}
