using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_CAMPAIGNS
    {
		[Key]
		public int CAMPAIGNID;
		public string CAMPAIGNNAME;
		public decimal CAMPAIGNFEE;
		public byte CAMPAIGNTYPE;
		public int CAMPAIGNFREEDURATION;
		public decimal CAMPAIGNDISCOUNTAMOUNT;
		public byte ISACTIVE;
		public byte ISDELETED;
		public byte RECBY;
		public DateTime RECDATE;
	}
}
