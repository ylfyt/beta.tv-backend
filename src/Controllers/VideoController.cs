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
        public async Task<ActionResult<ResponseDto<DataVideosResponseDto>>> GetVideo()
        {
            List<Video> videos = await _context.Videos.ToListAsync();
            var response = new ResponseDto<DataVideosResponseDto>
            {
                success = true,
                data = new DataVideosResponseDto
                {
                    videos = videos
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideoResponseDto>>> GetVideoById(int id)
        {
            var response = new ResponseDto<DataVideoResponseDto>
            {
                success = false,
                data = new DataVideoResponseDto
                {
                    video = null
                }
            };
            var idVideos = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (idVideos.Count != 1)
            {
                response.message = "Video not found";
                return NotFound(response);
            }
            response.data.video = idVideos[0];
            response.success = true;
            return Ok(response);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ResponseDto<DataVideosResponseDto>>> GetVideoByCategory(string category)
        {
            var response = new ResponseDto<DataVideosResponseDto>
            {
                success = false,
                data = new DataVideosResponseDto()
            };
            var catVideos = await _context.Videos.Where(v => v.Category == category).ToListAsync();
            if (catVideos.Count == 0)
            {
                response.message = "Videos not found";
                return BadRequest(response);
            }
            response.data.videos = catVideos;
            response.success = true;
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DataVideoResponseDto>>> UploadVideo([FromBody] VideoAddDto input)
        {

            var response = new ResponseDto<DataVideoResponseDto>
            {
                success = false,
                data = new DataVideoResponseDto
                {
                    video = null
                }
            };

            try
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

                response.success = true;
                response.data.video = insert;
                return Ok(response);
            }
            catch (System.Exception)
            {
                response.message = "Failed to add video";
                return BadRequest(response);
            }
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
            await _context.SaveChangesAsync();
            return Ok(deletedVideo[0]);
        }

    }
}
