namespace EsPark_WebApplication.Helper.DTO
{
    public class BorcSorgulaDetayRequest
    {

        public string Plaka { get; set; }
        public decimal Borc { get; set; }
        public decimal Alinan { get; set; }
        public List<ResultDatum> ResultData { get; set; }
        public int PARKINGDEBTID { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? OUTDATE { get; set; }
        public decimal? BORC { get; internal set; }
        public string LOCNAME { get; set; }
        public string REALNAME { get; set; }
        public string PLAKA { get; set; }

        public class ResultDatum
        {
            public int PARKINGDEBTID { get; set; }
            public DateTime? STARTDATE { get; set; }
            public DateTime? OUTDATE { get; set; }
            public string BORC { get; set; }
            public string LOCNAME { get; set; }
            public string REALNAME { get; set; }
        }

    }
}
