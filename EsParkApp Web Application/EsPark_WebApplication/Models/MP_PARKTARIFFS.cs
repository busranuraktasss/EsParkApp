using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_PARKTARIFFS
    {
		[Key]
		public int TARIFFID;
		public string TARIFFNAME;
		public int TOLERANCE;
		public decimal FIXEDENTRYFEE;
		public int FIXEDENTRYDURATION;
		public byte VALIDDAYS;
		public byte MONDAY;
		public byte TUESDAY;
		public byte WEDNESDAY;
		public byte THURSDAY;
		public byte FRIDAY;
		public byte SATURDAY;
		public byte SUNDAY;
		public decimal H00000030;
		public decimal H00300100;
		public decimal H01000130;
		public decimal H01300200;
		public decimal H02000230;
		public decimal H02300300;
		public decimal H03000330;
		public decimal H03300400;
		public decimal H04000430;
		public decimal H04300500;
		public decimal H05000530;
		public decimal H05300600;
		public decimal H06000630;
		public decimal H06300700;
		public decimal H07000730;
		public decimal H07300800;
		public decimal H08000830;
		public decimal H08300900;
		public decimal H09000930;
		public decimal H09301000;
		public decimal H10001030;
		public decimal H10301100;
		public decimal H11001130;
		public decimal H11301200;
		public decimal H12001230;
		public decimal H12301300;
		public decimal H13001330;
		public decimal H13301400;
		public decimal H14001430;
		public decimal H14301500;
		public decimal H15001530;
		public decimal H15301600;
		public decimal H16001630;
		public decimal H16301700;
		public decimal H17001730;
		public decimal H17301800;
		public decimal H18001830;
		public decimal H18301900;
		public decimal H19001930;
		public decimal H19302000;
		public decimal H20002030;
		public decimal H20302100;
		public decimal H21002130;
		public decimal H21302200;
		public decimal H22002230;
		public decimal H22302300;
		public decimal H23002330;
		public decimal H23302400;
		public byte ISACTIVE;
		public byte ISDELETED;
		public int RECBY;
		public DateTime RECDATE;
	}
}
