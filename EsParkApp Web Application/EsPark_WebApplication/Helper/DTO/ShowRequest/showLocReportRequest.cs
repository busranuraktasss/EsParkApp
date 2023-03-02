namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class showLocReportRequest
    {
        public string DT_ShiftId { get; set; }
        public long _Id { get; set; }
        public decimal? _toplanmasi_gereken { get; set; }
        public decimal? _toplanan { get; set; }
        public decimal? _toplanan_nakit { get; set; }
        public decimal? _toplanan_kkarti { get; set; }
        public decimal? _borc { get; set; }
        public decimal? _borc_nakit { get; set; }
        public decimal? _borc_kkarti { get; set; }
        public decimal? _genel_nakit { get; set; }
        public decimal? _genel_kkart { get; set; }
        public decimal? _toplanan_genel { get; set; }
        public string _firma { get; set; }
        public DateTime? _tarih { get; set; }
        public string _locname { get; set; }
        public string _username { get; set; }

    }
}
