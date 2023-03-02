using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_ZREPORTS
    {
        [Key]
		public int ID;
		public int JOBROTATIONHISTORYID;
		public DateTime CREATEDDATE;
		public DateTime? REPORTTAKENDATE;
		public int ZREPORTNUMBER;
		public decimal CUMULATIVESUM;
		public decimal CUMULATIVESUMVAT;
		public decimal? TOTALMONEYMUSTCOLLECTED;
		public decimal? TOTALMONEYCOLLECTED;
		public decimal? TOTALMONEYCOLLECTEDVAT;

        
    }
}
