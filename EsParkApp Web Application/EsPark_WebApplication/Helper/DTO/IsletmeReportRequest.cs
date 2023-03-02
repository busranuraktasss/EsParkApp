namespace EsPark_WebApplication.Helper.DTO
{
    public class IsletmeReportRequest
    {
        public string? Id { get; set; }
        public string? Toplanmasi_gereken { get; set; }
        public string? Toplanan { get; set; }
        public string? Toplanan_nakit { get; set; }
        public string? Toplanan_kkarti { get; set; }
        public string? Borc { get; set; }
        public string? Borc_nakit { get; set; }
        public string? Borc_kkarti { get; set; }
        public string? Genel_nakit { get; set; }
        public string? Genel_kkart { get; set; }
        public string? Toplanan_genel { get; set; }
        public string? Firma { get; set; }
        public string? Tarih { get; set; }
        public string Locname { get; set; }
        public string Username { get; set; }
    }
}
