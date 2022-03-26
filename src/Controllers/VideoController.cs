using Microsoft.AspNetCore.Mvc;
using src.Data;
using src.Models;
using src.Dtos;
using src.Interfaces;
using src.Filters;
using src.Dtos.video;
using Dtos.video;

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
        [AuthorizationCheckFilter]
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
            var idVideos = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (idVideos.Count != 1)
            {
                return NotFound(new ResponseDto<DataVideo>
                {
                    message = "Video not found"
                });
            }

            return Ok(new ResponseDto<DataVideo>
            {
                success = true,
                data = new DataVideo
                {
                    video = idVideos[0]
                }
            });
        }

        // [HttpGet("category/{category}")]
        // public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideoByCategory(string category)
        // {
        //     var catVideos = await _context.Videos.Where(v => v.Category == category).ToListAsync();
        //     if (catVideos.Count == 0)
        //     {
        //         return NotFound(new ResponseDto<DataVideos>
        //         {
        //             message = "Video not found",
        //             data = new DataVideos()
        //         });
        //     }

        //     return Ok(new ResponseDto<DataVideos>
        //     {
        //         success = true,
        //         data = new DataVideos
        //         {
        //             videos = catVideos
        //         }
        //     });
        // }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideo>>> UploadVideo([FromBody] VideoAddDto input)
        {
            try
            {
                var videoData = await GetVideoUsingYoutubeAPI(input.YoutubeVideoId);

                if (videoData == null || videoData.pageInfo?.resultsPerPage == 0)
                {
                    return BadRequest(
                        new ResponseDto<DataVideo>
                        {
                            message = "Failed to upload video"
                        }
                    );
                }

                var userAuth = HttpContext.Items["user"] as User;

                var insert = new Video
                {
                    YoutubeVideoId = input.YoutubeVideoId,
                    Title = videoData.items[0].snippet != null ? videoData.items[0].snippet!.title : "",
                    ThumbnailUrl = videoData.items[0].snippet.thumbnails.high.url,
                    ChannelId = videoData.items[0].snippet.channelId,
                    ChannelName = videoData.items[0].snippet.channelTitle,
                    Url = "https://www.youtube.com/embed/" + input.YoutubeVideoId,
                    Description = videoData.items[0].snippet.description,
                    Categories = input.Categories,
                    AuthorDescription = input.AuthorDescription,
                    AuthorTitle = input.AuthorTitle,
                    AuthorName = userAuth!.Id.ToString(),
                    AuthorId = userAuth.Id

                };

                await _context.Videos.AddAsync(insert);
                await _context.SaveChangesAsync();

                return Ok(new ResponseDto<DataVideo>
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
                return BadRequest(new ResponseDto<DataVideo>
                {
                    message = "Failed to add video"
                });
            }
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<ResponseDto<DataVideo>>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        // {
        //     try
        //     {
        //         var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
        //         if (selectedVideo.Count != 1)
        //         {
        //             return NotFound(new ResponseDto<DataVideo>
        //             {
        //                 message = "Video not found",
        //             });
        //         }
        //         selectedVideo[0].Title = input.Title;
        //         selectedVideo[0].Category = input.Category;
        //         selectedVideo[0].Description = input.Description;
        //         selectedVideo[0].Url = input.Url;
        //         await _context.SaveChangesAsync();

        //         return Ok(new ResponseDto<DataVideo>
        //         {
        //             success = true,
        //             data = new DataVideo
        //             {
        //                 video = selectedVideo[0]
        //             }
        //         });
        //     }
        //     catch (System.Exception)
        //     {
        //         return BadRequest(new ResponseDto<DataVideo>
        //         {
        //             message = "Failed to edit video"
        //         });
        //     }
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<DataVideo>>> DeleteVideo(int id)
        {
            var deletedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
            if (deletedVideo.Count != 1)
            {
                return NotFound(new ResponseDto<DataVideo>
                {
                    message = "Video not found"
                });
            }
            _context.Videos.Remove(deletedVideo[0]);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataVideo>
            {
                success = true,
                data = new DataVideo
                {
                    video = deletedVideo[0]
                }
            });
        }

        [HttpGet("woww")]
        public async Task<string> test()
        {
            var videoData = await GetVideoUsingYoutubeAPI("fCw2NZfR74E");
            if (videoData == null)
            {
                return "Wrong";
            }

            Console.WriteLine(videoData.items[0].snippet?.title);
            return "hello";
        }

        private async Task<YoutubeApiResponseDto?> GetVideoUsingYoutubeAPI(string id)
        {
            string YOUTUBE_API_BASE_URL = "https://www.googleapis.com/youtube/v3";
            string YOUTUBE_API_VIDEOS_ENDPOINT = "videos";
            string API_KEY = "AIzaSyDUJmNHHT7ruc3Tt4u8ITp0yzFkFDN_Fbg";
            string PART = "snippet";

            string requestUri = $"{YOUTUBE_API_BASE_URL}/{YOUTUBE_API_VIDEOS_ENDPOINT}?key={API_KEY}&id={id}&part={PART}";

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(requestUri);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Fetching youtube api failed...");
                    return null;
                }

                var responseBody = await response.Content.ReadFromJsonAsync<YoutubeApiResponseDto>();

                return responseBody;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Fetching youtube api failed...");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
