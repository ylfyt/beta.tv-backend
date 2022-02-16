namespace src.Models
{
    public class TokenLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}