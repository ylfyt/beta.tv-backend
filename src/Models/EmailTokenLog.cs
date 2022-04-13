namespace src.Models
{
    public class EmailTokenLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}