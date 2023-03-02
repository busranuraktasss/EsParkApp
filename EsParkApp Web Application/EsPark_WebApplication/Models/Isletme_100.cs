using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Xml.Linq;

namespace EsPark_WebApplication.Models
{
    public class Isletme_100
    {
        private DateTime _backingForCREATEDDATE { get; set; }
        private string _backingForLOCNAME { get; set; }
        private string _backingForSERIALNO { get; set; }
        private string _backingForREALNAME { get; set; }
        private DateTime _backingForBEGINDATE { get; set; }
        private decimal? _backingForGereken { get; set; }
        private decimal? _backingForToplanan { get; set; }
        private decimal? _backingForKacanPara { get; set; }
        private int _backingForID { get; set; }
        private decimal? _backingForBorc { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public string LOCNAME { get; set; }
        public string SERIALNO { get; set; }
        public string REALNAME { get; set; }
        public DateTime BEGINDATE { get; set; }
        public decimal? Gereken { get; set; }
        public decimal? Toplanan { get; set; }
        public decimal? KacanPara { get; set; }
        public int ID { get; set; }
        public decimal? Borc { get; set; }
    }
}
