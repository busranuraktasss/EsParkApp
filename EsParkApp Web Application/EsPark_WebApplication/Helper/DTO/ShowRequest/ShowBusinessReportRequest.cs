namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class ShowBusinessReportRequest
    {
        public int _pts_otopark_adedi { get; set; }
        public int _pts_parkeden_arac { get; set; }
        public int _pts_parkeden_abone { get; set; }
        public int _pts_personel_adedi { get; set; }
        public int _pts_odeyenler { get; set; }
        public int _pts_borclu { get; set; }
        public decimal _gereken { get; set; }
        public decimal _toplanan { get; set; }
        public decimal _toplanamayan { get; set; }
    }
}
