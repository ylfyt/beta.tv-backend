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
        [AuthorizationCheckFilter]
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

        [HttpGet("category/{category}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideoByCategory(string category)
        {
            var catVideos = await _context.Videos.Where(v => v.Categories.Contains(category)).ToListAsync();
            if (catVideos.Count == 0)
            {
                return NotFound(new ResponseDto<DataVideos>
                {
                    message = "Video not found",
                    data = new DataVideos()
                });
            }

            return Ok(new ResponseDto<DataVideos>
            {
                success = true,
                data = new DataVideos
                {
                    videos = catVideos
                }
            });
        }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideo>>> UploadVideo([FromBody] VideoAddDto input)
        {
            try
            {
                var youtubeResponse = await GetVideoUsingYoutubeAPI(input.YoutubeVideoId);

                if (youtubeResponse == null || youtubeResponse.pageInfo?.resultsPerPage == 0)
                {
                    return BadRequest(
                        new ResponseDto<DataVideo>
                        {
                            message = "Failed to upload video"
                        }
                    );
                }

                var videoData = youtubeResponse.items[0];

                var userAuth = HttpContext.Items["user"] as User;

                var insert = new Video
                {
                    YoutubeVideoId = input.YoutubeVideoId,
                    Title = videoData.snippet != null ? videoData.snippet!.title : "",
                    ThumbnailUrl = videoData.snippet!.thumbnails!.high!.url,
                    ChannelId = videoData.snippet.channelId,
                    ChannelName = videoData.snippet.channelTitle,
                    Url = "https://www.youtube.com/embed/" + input.YoutubeVideoId,
                    Description = videoData.snippet.description,
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

        [HttpPut("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideo>>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        {
            try
            {
                var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
                if (selectedVideo.Count != 1)
                {
                    return NotFound(new ResponseDto<DataVideo>
                    {
                        message = "Video not found",
                    });
                }

                var userAuth = HttpContext.Items["user"] as User;

                if (selectedVideo[0].AuthorId != userAuth!.Id)
                {
                    return BadRequest(new ResponseDto<DataVideo>
                    {
                        message = "You are not allowed to edit this video!",
                    });
                }

                if (input.Categories.Count == 0)
                {
                    if (input.AuthorDescription == "" && input.AuthorTitle == "")
                    {
                        return BadRequest(new ResponseDto<DataVideo>
                        {
                            message = "Cannot empty",
                        });
                    }
                }

                selectedVideo[0].AuthorTitle = input.AuthorTitle;
                selectedVideo[0].Categories = input.Categories;
                selectedVideo[0].AuthorDescription = input.AuthorDescription;

                await _context.SaveChangesAsync();

                return Ok(new ResponseDto<DataVideo>
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
                return BadRequest(new ResponseDto<DataVideo>
                {
                    message = "Failed to edit video"
                });
            }
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter]
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

        [HttpPost("search")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideos>>> GetVideoByQuery([FromBody] SearchVideoDto input)
        {
            if (input.Query == "")
            {
                return BadRequest(new ResponseDto<DataVideos>
                {
                    message = "Please input query"
                });
            }

            var arr = input.Query.ToLower().Split(' ');

            List<Video> videos = new List<Video>();
            foreach (var word in arr)
            {
                var newVideos = _context.Videos.AsEnumerable().Where(a => a.Title.ToLower().Contains(word)).Except(videos).ToList();
                videos.AddRange(newVideos);
            }

            return Ok(new ResponseDto<DataVideos>
            {
                success = true,
                data = new DataVideos
                {
                    videos = videos
                }
            });
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
