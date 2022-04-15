﻿using Microsoft.AspNetCore.Http;
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
            // add : get user's own bookmarks
            var userAuth = HttpContext.Items["user"] as User;
            var user = await _context.User.FindAsync(userAuth!.Id);
            if (user == null)
            {
                return NotFound(new ResponseDto<DataVideos>
                {
                    message = "User Not Found"
                });
            }

            var bookmarks = await _context.Bookmarks.Where(v => v.UserId == user.Id).ToListAsync();
            List<Video> videoList = new List<Video>();
            foreach (var bookmark in bookmarks)
            {
                var video = await _context.Videos.Where(v => v.Id == bookmark.VideoId).ToListAsync();
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

        [HttpGet("isBookmarked")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<bool>>>GetIsBookmarked(int id){

            try{
                var userAuth = HttpContext.Items["user"] as User;
                var user = await _context.User.FindAsync(userAuth!.Id);

                var targetBookmark = await _context.Bookmarks.Where(v => v.Id == id && v.UserId == user.Id).ToListAsync();
                if (targetBookmark.Count > 0){
                    var response = new ResponseDto<bool>
                    {
                        success = true,
                        data = true
                    };
                    return Ok(response);
                }
                else{
                    var response = new ResponseDto<bool>
                    {
                        success = true,
                        data = false
                    };
                    return Ok(response);
                }
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseDto<DataBookmark>
                {
                    message = "Failed to add bookmark"
                });
            }
            
        }

        [HttpGet("{id}")]
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
            var video = await _context.Videos.Where(v => v.Id == targetBookmark[0].VideoId).ToListAsync();
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
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> AddBookmark([FromBody] BookmarkAdd input)
        {
            try
            {
                var video = await _context.Videos.FindAsync(input.VideoId);
                if (video == null)
                {
                    return NotFound(new ResponseDto<DataBookmark>
                    {
                        message = "Video Not Found"
                    });
                }

                var user = HttpContext.Items["user"] as User;

                var bookmarkExist = await _context.Bookmarks.Where(b => b.UserId == user.Id && b.VideoId == input.VideoId).FirstOrDefaultAsync();

                if (bookmarkExist != null)
                {
                    return BadRequest(new ResponseDto<DataBookmark>
                    {
                        message = "Already bookmarked this video"
                    });
                }

                var insert = new Bookmark
                {
                    VideoId = input.VideoId,
                    UserId = user!.Id
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
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> DeleteBookmark(int id)
        {
            var deletedBookmark = await _context.Bookmarks.FindAsync(id);

            if (deletedBookmark == null)
            {
                return NotFound(new ResponseDto<DataBookmark>
                {
                    message = "Video not found"
                });
            }
            var user = HttpContext.Items["user"] as User;

            if (deletedBookmark.UserId != user!.Id)
            {
                return Unauthorized(new ResponseDto<DataBookmark>
                {
                    message = "Unauthorized to delete this bookmark"
                });
            }

            _context.Bookmarks.Remove(deletedBookmark);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataBookmark>
            {
                success = true,
                data = new DataBookmark
                {
                    bookmark = deletedBookmark
                }
            });
        }


    }
}
