using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EsPark_WebApplication.Helper.DTO
{
    public class shiftMapRequest
    {

        public string Lat { get; set; }

        public string Lng { get; set; }

        public string JobId { get; set; }

        public DateTime CreatedTime { get; set; }


    }
}
