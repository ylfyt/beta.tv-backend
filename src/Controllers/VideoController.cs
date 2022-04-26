using Microsoft.AspNetCore.Mvc;
using src.Data;
using src.Models;
using src.Dtos;
using src.Filters;
using src.Dtos.video;
using Dtos.video;

using One = src.Dtos.ResponseDto<src.Dtos.video.DataVideo>;
using Many = src.Dtos.ResponseDto<src.Dtos.video.DataVideos>;

namespace if3250_2022_01_buletin_backend.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : Controller
    {
        public readonly DataContext _context;
        private readonly IResponseGetter<DataVideo> _responseGetterSingle;
        private readonly IResponseGetter<DataVideos> _responseGetterMany;

        public VideoController(DataContext context, IResponseGetter<DataVideo> responseGetterSingle, IResponseGetter<DataVideos> responseGetterMany)
        {
            _context = context;
            _responseGetterSingle = responseGetterSingle;
            _responseGetterMany = responseGetterMany;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GetVideo()
        {
            List<Video> videos = await _context.Videos.Include(v => v.Categories).ToListAsync();

            var response = _responseGetterMany.Success(new DataVideos
            {
                videos = videos
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> GetVideoById(int id)
        {
            var video = await _context.Videos.Include(v => v.Categories).FirstOrDefaultAsync(v => v.Id == id);
            if (video == null)
            {
                return NotFound(_responseGetterSingle.Error("Video not found"));
            }

            return Ok(_responseGetterSingle.Success(new DataVideo
            {
                video = video
            }));
        }

        [HttpGet("category/{category}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GetVideoByCategory(string category)
        {
            var catVideos = await _context.Categories.Where(c => c.Slug == category).Include(c => c.Videos).ToListAsync();

            if (catVideos.Count == 0)
            {
                return NotFound(_responseGetterMany.Error("Category not found!"));
            }

            return Ok(_responseGetterMany.Success(new DataVideos
            {
                videos = catVideos[0].Videos
            }));
        }

        [HttpPost]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> UploadVideo([FromBody] VideoAddDto input)
        {
            try
            {
                var tempVideo = await _context.Videos.Where(v => v.YoutubeVideoId == input.YoutubeVideoId).ToListAsync();

                if (tempVideo.Count != 0)
                {
                    return BadRequest(_responseGetterSingle.Error("Video Already Exist"));
                }

                var categories = new List<Category>();
                foreach (var slug in input.CategorySlugs)
                {
                    var category = await _context.Categories.Where(c => c.Slug == slug).FirstOrDefaultAsync();
                    if (category == null)
                    {
                        return BadRequest(_responseGetterSingle.Error($"Category {slug} doesn't exist"));
                    }
                    categories.Add(category);
                }


                var youtubeResponse = await GetVideoUsingYoutubeAPI(input.YoutubeVideoId);

                if (youtubeResponse == null || youtubeResponse.pageInfo?.resultsPerPage == 0)
                {
                    return BadRequest(_responseGetterSingle.Error("Failed to upload video"));
                }

                if (youtubeResponse?.items[0]?.snippet?.channelId != Constants.YOUTUBE_CHANNEL_ID)
                {
                    return BadRequest(_responseGetterSingle.Error("The video is not from beta.tv youtube channel"));
                }

                var videoData = youtubeResponse.items[0];

                var userAuth = HttpContext.Items["user"] as User;

                var insert = new Video
                {
                    YoutubeVideoId = input.YoutubeVideoId,
                    Title = videoData.snippet != null ? videoData.snippet!.title : "",
                    ThumbnailUrl = videoData.snippet!.thumbnails!.medium!.url,
                    ChannelId = videoData.snippet.channelId,
                    ChannelName = videoData.snippet.channelTitle,
                    Url = "https://www.youtube.com/embed/" + input.YoutubeVideoId,
                    Description = videoData.snippet.description,
                    Categories = categories,
                    AuthorDescription = input.AuthorDescription,
                    AuthorTitle = input.AuthorTitle,
                    AuthorName = userAuth!.Name,
                    AuthorId = userAuth.Id

                };

                await _context.Videos.AddAsync(insert);
                await _context.SaveChangesAsync();

                return Ok(_responseGetterSingle.Success(new DataVideo
                {
                    video = insert
                }));
            }
            catch (System.Exception)
            {
                return BadRequest(_responseGetterSingle.Error("Failed to upload video"));
            }
        }

        [HttpPut("{id}")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> UpdateVideo(int id, [FromBody] VideoUpdateDto input)
        {
            try
            {
                var selectedVideo = await _context.Videos.Where(v => v.Id == id).ToListAsync();
                if (selectedVideo.Count != 1)
                {
                    return NotFound(_responseGetterSingle.Error("Video not found"));
                }

                var userAuth = HttpContext.Items["user"] as User;

                if (selectedVideo[0].AuthorId != userAuth!.Id)
                {
                    return Unauthorized(_responseGetterSingle.Error("You are not allowed to edit this video!"));
                }

                if (input.AuthorDescription == "" || input.AuthorTitle == "")
                {
                    return BadRequest(_responseGetterSingle.Error("Title or Description Cannot empty"));
                }

                var categories = new List<Category>();
                foreach (var slug in input.CategorySlugs)
                {
                    var category = await _context.Categories.Where(c => c.Slug == slug).FirstOrDefaultAsync();
                    if (category == null)
                    {
                        return BadRequest(_responseGetterSingle.Error($"Category {slug} doesn't exist"));
                    }
                    categories.Add(category);
                }


                selectedVideo[0].AuthorTitle = input.AuthorTitle;
                selectedVideo[0].AuthorDescription = input.AuthorDescription;
                selectedVideo[0].Categories = categories;

                await _context.SaveChangesAsync();

                return Ok(_responseGetterSingle.Success(new DataVideo
                {
                    video = selectedVideo[0]
                }));
            }
            catch (System.Exception)
            {
                return BadRequest(_responseGetterSingle.Error("Failed to edit video"));
            }
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> DeleteVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound(_responseGetterSingle.Error("Video not found"));
            }
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataVideo
            {
                video = video
            }));
        }

        [HttpPost("search")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GetVideoByQuery([FromBody] SearchVideoDto input)
        {
            if (input.Query == "")
            {
                return BadRequest(_responseGetterMany.Error("Please input query"));
            }

            var arr = input.Query.ToLower().Split(' ');

            List<Video> videos = new List<Video>();
            foreach (var word in arr)
            {
                var newVideos = _context.Videos.Include(v => v.Categories).AsEnumerable().Where(a => a.Title.ToLower().Contains(word)).Except(videos).ToList();
                videos.AddRange(newVideos);
            }

            return Ok(_responseGetterMany.Success(new DataVideos
            {
                videos = videos
            }));
        }

        private async Task<YoutubeApiResponseDto?> GetVideoUsingYoutubeAPI(string id)
        {
            string YOUTUBE_API_BASE_URL = "https://www.googleapis.com/youtube/v3";
            string YOUTUBE_API_VIDEOS_ENDPOINT = "videos";
            string API_KEY = ServerInfo.GOOGLE_API_KEY;
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
