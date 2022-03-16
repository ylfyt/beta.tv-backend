using Microsoft.AspNetCore.Mvc;
using src.Data;
using src.Models;
using src.Dtos;
using src.Interfaces;
using src.Filters;


namespace if3250_2022_01_buletin_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : Controller
    {

        private static List<Video> videos = new List<Video> { };

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetVideo() 
        { 
            return Ok(videos);                
        }
        
        [HttpGet("{category}")]
        public async Task<ActionResult<List<Video>>> GetVideo(string category) 
        { 
            var catVideos = videos.Find(v => v.Category == category);
            if (catVideos == null)
            {
                return BadRequest("There is no such category");
            }
            return Ok(catVideos);
        }
        
        [HttpPost]
        public async Task<ActionResult<List<Video>>> UploadVideo(Video vid)
        {
            videos.Add(vid);
            return Ok(videos);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Video>>> UpdateVideo(int id, string name, string category, string channel, string url)
        {
            var selectedVideo = videos.Find(v => v.Id == id);
            if (selectedVideo == null)
            {
                return BadRequest("Index out of bounds");
            }
            selectedVideo.Name = name;
            selectedVideo.Category = category;
            selectedVideo.Channel = channel;
            selectedVideo.Url = url;
            
            return Ok(videos);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Video>>> DeleteVideo(int id)
        {
            var deletedVideo = videos.Find(v => v.Id == id);
            if (deletedVideo == null)
            {
                return BadRequest("There is no such video");
            }
            videos.Remove(deletedVideo);
            return Ok(videos);
        }
        
    }
}
