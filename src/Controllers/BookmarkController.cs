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
        public async Task<ActionResult<ResponseDto<DataBookmark>>> GetBookmark()
        {
            List<Bookmark> bookmark = await _context.Bookmark.ToListAsync();
            var response = new ResponseDto<DataBookmark>
            {
                success = true,
                data = new DataBookmark
                {
                    bookmark = bookmark
                }
            };
            return Ok(response);
        }

        [HttpGet("{Id_Bookmark}")]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> GetBookmarkById(int id)
        {
            var targetBookmark = await _context.Bookmark.Where(v => v.Id_Bookmark == id).ToListAsync();
            if (targetBookmark.Count != 1)
            {
                return NotFound(new ResponseDto<DataBookmark>
                {
                    message = "Video not found"
                });
            }
            var response = new ResponseDto<DataBookmark>
            {
                success = true,
                data = new DataBookmark
                {
                    bookmark = targetBookmark
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

                await _context.Bookmark.AddAsync(insert);
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

        [HttpDelete("{Id_Bookmark}")]
        public async Task<ActionResult<ResponseDto<DataBookmark>>> DeleteBookmark(int id)
        {
            var deletedBookmark = await _context.Bookmark.Where(v => v.Id_Bookmark == id).ToListAsync();
            if (deletedBookmark.Count != 1)
            {
                return NotFound(new ResponseDto<DataBookmark>
                {
                    message = "Video not found"
                });
            }
            _context.Bookmark.Remove(deletedBookmark[0]);
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
