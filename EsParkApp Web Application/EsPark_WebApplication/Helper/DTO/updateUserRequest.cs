namespace EsPark_WebApplication.Helper.DTO.UpdateRequest
{
    public class updateUserRequest
    {
        public int updateId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Realname { get; set; }
        public string Phone { get; set; }
        public int Durum { get; set; }
        public int Authority { get; set; }
        //
    }
}
