using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Subscriber_Count { get; set; }
    }
}
