using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Data;
using src.Models;
using src.Dtos;
using src.Dtos.bookmark;
using src.Dtos.video;
using src.Filters;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : Controller
    {
        // GET: BookmarkController
        public readonly DataContext _context;

        public BookmarkController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideo>>> GetBookmark()
        {
            var bookmarks = await _context.Bookmarks.ToListAsync();
            List<Video> videoList = new List<Video>();
            foreach (var bookmark in bookmarks)
            {
                var video = await _context.Videos.Where(v => v.Id == bookmark.Id_Video).ToListAsync();
                if (video.Count != 1)
                {
                    continue;
                }
                videoList.Add(video[0]);
            }
            var response = new ResponseDto<DataVideos>
            {
                success = true,
                data = new DataVideos
                {
                    videos = videoList
                }
            };
            return Ok(response);
        }

        [HttpGet("{Id_Bookmark}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataVideo>>> GetBookmarkById(int id)
        {
            var targetBookmark = await _context.Bookmarks.Where(v => v.Id == id).ToListAsync();
            if (targetBookmark.Count != 1)
            {
                return NotFound(new ResponseDto<DataBookmark>
                {
                    message = "Bookmark not found"
                });
            }
            var video = await _context.Videos.Where(v => v.Id == targetBookmark[0].Id_Video).ToListAsync();
            if (video.Count != 1)
            {
                return NotFound(new ResponseDto<DataVideo>
                {
                    message = "Video not found"
                });
            }
            var response = new ResponseDto<DataVideo>
            {
                success = true,
                data = new DataVideo
                {
                    video = video[0]
                }
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> AddBookmark([FromBody] BookmarkAdd input)
        {
            try
            {
                var insert = new Bookmark
                {
                    ChannelId = input.ChannelId,
                    Id_Video = input.Id_Video,
                    Id_User = input.Id_User
                };

                await _context.Bookmarks.AddAsync(insert);
                await _context.SaveChangesAsync();

                return Ok(new ResponseDto<DataBookmark>
                {
                    success = true,
                    data = new DataBookmark
                    {
                        bookmark = insert
                    }
                });
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseDto<DataBookmark>
                {
                    message = "Failed to add bookmark"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> DeleteBookmark(int id)
        {
            var deletedBookmark = await _context.Bookmarks.Where(v => v.Id == id).ToListAsync();
            if (deletedBookmark.Count != 1)
            {
                return NotFound(new ResponseDto<DataBookmark>
                {
                    message = "Video not found"
                });
            }
            _context.Bookmarks.Remove(deletedBookmark[0]);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataBookmark>
            {
                success = true,
                data = new DataBookmark
                {
                    bookmark = deletedBookmark[0]
                }
            });
        }
    }
}
