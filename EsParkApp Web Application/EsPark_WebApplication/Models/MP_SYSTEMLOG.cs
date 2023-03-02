using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_SYSTEMLOG
    {
		[Key]
		public int ROWID;
		public string OPTYPE;
		public DateTime OPDATE;
		public string STNAME;
		public int RECBY;
		public string CARDNO;
		public string EXP;
	}
}
