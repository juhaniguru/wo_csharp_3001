namespace API.Dtos
{
    public class LoginReq
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}