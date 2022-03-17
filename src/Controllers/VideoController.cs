using Microsoft.AspNetCore.Mvc;
using src.Data;
using src.Models;
using src.Dtos;
using src.Interfaces;
using src.Filters;
using src.Dtos.video;

namespace if3250_2022_01_buletin_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : Controller
    {
        public readonly DataContext _context;
        
        public VideoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Video>>> GetVideo()
        {
            List<Video> videos = await _context.Videos.ToListAsync();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var idVideos = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (idVideos.Count != 1)
            {
                return BadRequest("Video not found");
            }
            return Ok(idVideos[0]);
        }

        [HttpGet("{category}")]
        public async Task<ActionResult<List<Video>>> GetVideo(string category)
        {
            var catVideos = await _context.Videos.Where(v => v.Category == category).ToListAsync();
            if (catVideos.Count == 0)
            {
                return BadRequest("There is no such category");
            }
            return Ok(catVideos);
        }

        [HttpPost]
        public async Task<ActionResult<Video>> UploadVideo([FromBody] VideoAddDto input)
        {
            var insert = new Video
            {
                Title = input.Title,
                ChannelId = input.ChannelId,
                Url = input.Url,
                Views = input.Views,
                Rating = input.Rating,
                Category = input.Category,
                Description = input.Description,
                Release_Date = new DateTime()
            };

            await _context.Videos.AddAsync(insert);
            await _context.SaveChangesAsync();
            return Ok(insert);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Video>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        {
            var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (selectedVideo.Count != 1)
            {
                return BadRequest("Error fetching video");
            }
            selectedVideo[0].Title = input.Title;
            selectedVideo[0].Category = input.Category;
            selectedVideo[0].Description = input.Description;
            selectedVideo[0].Url = input.Url;
            await _context.SaveChangesAsync();

            return Ok(selectedVideo[0]);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Video>>> DeleteVideo(int id)
        {
            var deletedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (deletedVideo.Count != 1)
            {
                return BadRequest("There is no such video");
            }
            _context.Videos.Remove(deletedVideo[0]);
            return Ok(deletedVideo[0]);
        }

    }
}
