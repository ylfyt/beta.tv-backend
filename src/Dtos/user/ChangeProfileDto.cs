namespace src.Dtos
{
    public class ChangeProfileDto
    {
        public string OldUsername {get; set;} = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}