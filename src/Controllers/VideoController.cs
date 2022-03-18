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
            var response = new ResponseDto
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
            var idVideos = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (idVideos.Count != 1)
            {
                return NotFound(new ResponseDto
                {
                    message = "Video not found"
                });
            }

            return Ok(new ResponseDto
            {
                success = true,
                data = new DataVideo
                {
                    video = idVideos[0]
                }
            });
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideoByCategory(string category)
        {
            var catVideos = await _context.Videos.Where(v => v.Category == category).ToListAsync();
            if (catVideos.Count == 0)
            {
                return NotFound(new ResponseDto
                {
                    message = "Video not found",
                    data = new DataVideos()
                });
            }

            return Ok(new ResponseDto
            {
                success = true,
                data = new DataVideos
                {
                    videos = catVideos
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DataVideo>>> UploadVideo([FromBody] VideoAddDto input)
        {
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

                return Ok(new ResponseDto
                {
                    success = true,
                    data = new DataVideo
                    {
                        video = insert
                    }
                });
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseDto
                {
                    message = "Failed to add video"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideo>>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        {
            try
            {
                var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
                if (selectedVideo.Count != 1)
                {
                    return NotFound(new ResponseDto
                    {
                        message = "Video not found",
                    });
                }
                selectedVideo[0].Title = input.Title;
                selectedVideo[0].Category = input.Category;
                selectedVideo[0].Description = input.Description;
                selectedVideo[0].Url = input.Url;
                await _context.SaveChangesAsync();

                return Ok(new ResponseDto
                {
                    success = true,
                    data = new DataVideo
                    {
                        video = selectedVideo[0]
                    }
                });
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseDto
                {
                    message = "Failed to edit video"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideo>>> DeleteVideo(int id)
        {
            var deletedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (deletedVideo.Count != 1)
            {
                return NotFound(new ResponseDto
                {
                    message = "Video not found"
                });
            }
            _context.Videos.Remove(deletedVideo[0]);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto
            {
                success = true,
                data = new DataVideo
                {
                    video = deletedVideo[0]
                }
            });
        }

    }
}
