namespace EsPark_WebApplication.Helper.DTO.AddRequest
{
    public class addUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public string Phone { get; set; }
        public int Durum { get; set; }
        public int Authority { get; set; }
    }
}
