using System.Text.Json.Serialization;

namespace src.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Video> Videos { get; set; } = new List<Video>();
    }
}