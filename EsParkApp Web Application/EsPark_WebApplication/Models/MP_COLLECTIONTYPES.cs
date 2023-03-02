using System.ComponentModel.DataAnnotations;

namespace EsPark_WebApplication.Models
{
    public class MP_COLLECTIONTYPES
    {
		[Key]
		public int COLLECTIONTYPEID;
		public string COLLECTIONTYPENAME;
		public string COLLECTIONTYPEDESCRIPTION;
	}
}
