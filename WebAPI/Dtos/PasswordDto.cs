namespace WebAPI.Dtos
{
    public class PasswordDto
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
