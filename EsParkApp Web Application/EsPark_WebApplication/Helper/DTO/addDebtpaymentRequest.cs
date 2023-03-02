namespace EsPark_WebApplication.Helper.DTO
{
    public class addDebtpaymentRequest
    {
		public decimal silinecekBorcTutarı { get; set; }	
		public string aciklama { get; set; }
		public int[] parkingIds { get; set; }
		public int deptpaymentId { get; set; }	

	}
}
