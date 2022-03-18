using src.Models;

namespace src.Dtos.video
{
    public class DataVideosResponseDto
    {
        public List<Video> videos { get; set; } = new List<Video>();
    }
}