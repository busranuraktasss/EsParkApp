namespace EsPark_WebApplication.Models
{
    public class MET_INCELENEN
    {
        public int Id { get; set; }
        public string TOKEN { get; set; }
        public string PLAKA { get; set; }
        public string PLAKA_ILK { get; set; }
        public DateTime ZAMAN { get; set; }
        public byte YON { get; set; }
        public int? SAHIP_ID { get; set; }
        public int? ARKA_INCELENEN_ID { get; set; }
        public int? KULLANICI_ID { get; set; }
        public string TIP { get; set; }
        public int? ARAC_ID { get; set; }
        public string PLAKALAR { get; set; }
        public int? CIKIS_ID { get; set; }
        public int? PARKLANMA { get; set; }
        public int MET_ID { get; set; }
    }
}
