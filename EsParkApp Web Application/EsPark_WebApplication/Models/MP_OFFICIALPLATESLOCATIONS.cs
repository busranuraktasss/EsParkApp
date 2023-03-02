using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsPark_WebApplication.Models
{
    public class MP_OFFICIALPLATESLOCATIONS
    {
		[Key]
		public int Id;

		[ForeignKey("MP_LOCATIONS")]
		public int LocId { get; set; }

        [ForeignKey("mp_OFFICIALPLATES")]
		public int PlatesId { get; set; }


	}
}
