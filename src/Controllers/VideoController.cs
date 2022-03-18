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
        public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideo()
        {
            List<Video> videos = await _context.Videos.ToListAsync();
            var response = new ResponseDto<DataVideos>
            {
                success = true,
                data = new DataVideos
                {
                    videos = videos
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideo>>> GetVideoById(int id)
        {
            var response = new ResponseDto<DataVideo>
            {
                success = false,
                data = new DataVideo
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
        public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideoByCategory(string category)
        {
            var response = new ResponseDto<DataVideos>
            {
                success = false,
                data = new DataVideos()
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
        public async Task<ActionResult<ResponseDto<DataVideo>>> UploadVideo([FromBody] VideoAddDto input)
        {

            var response = new ResponseDto<DataVideo>
            {
                success = false,
                data = new DataVideo
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
        public async Task<ActionResult<ResponseDto<DataVideo>>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        {
            var response = new ResponseDto<DataVideo>
            {
                success = false,
                data = new DataVideo
                {
                    video = null
                }
            };
            try
            {
                var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
                if (selectedVideo.Count != 1)
                {
                    response.message = "Video Not Found";
                    return NotFound(response);
                }
                selectedVideo[0].Title = input.Title;
                selectedVideo[0].Category = input.Category;
                selectedVideo[0].Description = input.Description;
                selectedVideo[0].Url = input.Url;
                await _context.SaveChangesAsync();

                response.data.video = selectedVideo[0];
                return Ok(response);
            }
            catch (System.Exception)
            {
                response.message = "Failed to add video";
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideo>>> DeleteVideo(int id)
        {
            var response = new ResponseDto<DataVideo>
            {
                success = false,
                data = new DataVideo
                {
                    video = null
                }
            };

            var deletedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (deletedVideo.Count != 1)
            {
                response.message = "Video Not Found";
                return NotFound(response);
            }
            _context.Videos.Remove(deletedVideo[0]);
            await _context.SaveChangesAsync();
            response.data.video = deletedVideo[0];
            return Ok(response);
        }

    }
}
