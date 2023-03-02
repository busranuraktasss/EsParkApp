namespace EsPark_WebApplication.Helper.DTO
{
    public class isletme
    {
        public int Id { get; set; }
        public int user_count { get; set; }
        public int lok_count { get; set; }
        public int arac_count { get; set; }
        public int odeyenler_count { get; set; }
        public int odemeyenler_count { get; set; }
        public int borclu_count { get; set; }
        public decimal? toplanmasi_gereken { get; set; }
        public decimal? toplanan { get; set; }
        public decimal? toplanan_nakit { get; set; }
        public decimal? toplanan_kkarti { get; set; }
        public decimal? borc { get; set; }
        public decimal? borc_nakit { get; set; }
        public decimal? borc_kkarti { get; set; }
        public decimal? toplanan_genel { get; set; }
        public decimal? genel_nakit { get; set; }
        public decimal? genel_kkart { get; set; }
        public string firma { get; set; }
        public string tarih { get; set; }
        public string tarih2 { get; set; }
    }
}
