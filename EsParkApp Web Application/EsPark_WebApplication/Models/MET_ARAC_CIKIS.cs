namespace EsPark_WebApplication.Models
{
    public class MET_ARAC_CIKIS
    {
        public int Id { get; set; }
        public int CIKIS_ID { get; set; }
        public string PLAKA { get; set; }
        public string GIRIS_PLAKA_ILK { get; set; }
        public string CIKIS_PLAKA_ILK { get; set; }
        public DateTime? GIRIS_ZAMANI { get; set; }
        public DateTime? CIKIS_ZAMANI { set; get; }
        public int? ABONE_ID { get; set; }
        public int? SURE { get; set; }
        public decimal? UCRET { get; set; }
        public int? KULLANICI_ID { get; set; }
        public string ACIKLAMA { get; set; }
        public int? CIKIS_INCELENEN_ID { get; set; }
        public decimal? BAKIYE { get; set; }
        public byte? ODEME_SEKLI_ID { get; set; }
        public decimal? SABIT_UCRET { get; set; }
        public string TOKEN { get; set; }
    }
}
