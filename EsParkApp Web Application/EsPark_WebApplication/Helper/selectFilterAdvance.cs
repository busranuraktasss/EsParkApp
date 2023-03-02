using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EsPark_WebApplication.Helper
{
    public class selectFilterAdvance
    {
        public int loc { get; set; }
        public int row { get; set; }
        public string plate { get; set; }   

        public int device { get; set; }
        public int fee_plate { get; set; }
        public int check { get; set; }
    }
}
