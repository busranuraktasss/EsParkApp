namespace EsPark_WebApplication.Helper.DTO.UpdateRequest
{
    public class updateParkingDebtsTransferRequest
    {
        public string aktaranPlaka { get; set; }
        public string aktarılanPlaka { get; set; }
        public int aktarılanPlakaId { get; set; }
        public int[] plakaIds { get; set; }

    }
}
