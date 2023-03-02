namespace EsPark_WebApplication.Helper.DTO.ShowRequest
{
    public class showTariffRequest
    {
        public string DT_TariffId { get; set; }
        public int tariffid { get; set; }
        public string tariffname { get; set; }
        public int tolerance { get; set; }
        public decimal fixedentryfee { get; set; }
        public int fixedentryduration { get; set; }
        public DateTime recdate { get; set; }
    }
}