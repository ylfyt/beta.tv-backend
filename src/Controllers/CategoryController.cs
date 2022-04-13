using src.Data;
using Microsoft.AspNetCore.Mvc;
using src.Dtos.category;
using src.Filters;
using System.Text.RegularExpressions;
using System.Text;
using src.Models;

using One = src.Dtos.ResponseDto<src.Dtos.category.DataCategory>;
using Many = src.Dtos.ResponseDto<src.Dtos.category.DataCategories>;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IResponseGetter<DataCategory> _responseGetterSingle;
        private readonly IResponseGetter<DataCategories> _responseGetterMany;
        public CategoryController(DataContext context, IResponseGetter<DataCategory> responseGetterSingle, IResponseGetter<DataCategories> responseGetterMany)
        {
            _context = context;
            _responseGetterSingle = responseGetterSingle;
            _responseGetterMany = responseGetterMany;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GET()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_responseGetterMany.Success(new DataCategories
            {
                categories = categories
            }));
        }

        [HttpGet("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> GET(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(_responseGetterSingle.Error());
            }

            return Ok(_responseGetterSingle.Success(new DataCategory
            {
                category = category
            }));
        }

        [HttpPost]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> POST([FromBody] CreateCategoryDto input)
        {
            var slug = GenerateSlug(input.Label);
            var category = await _context.Categories.Where(c => c.Slug == slug).FirstOrDefaultAsync();

            if (category != null)
            {
                return BadRequest(_responseGetterSingle.Error("Category already exist"));
            }

            var newCategory = new Category
            {
                Label = input.Label,
                Slug = slug
            };

            await _context.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataCategory
            {
                category = newCategory
            }));
        }

        [HttpPut("{id}")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> PUT(int id, [FromBody] CreateCategoryDto input)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(_responseGetterSingle.Error("Category not found"));
            }

            var slug = GenerateSlug(input.Label);

            category.Label = input.Label;
            category.Slug = slug;

            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataCategory
            {
                category = category
            }));
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<One>> DELETE(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(_responseGetterSingle.Error("Category not found"));
            }
            _context.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataCategory
            {
                category = category
            }));
        }

        private string GenerateSlug(string label, int maxLength = 45)
        {
            string str = RemoveAccent(label).ToLower();
            // remove invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // max length
            str = str.Substring(0, Math.Min(maxLength, str.Length));
            // space to hyphens
            str = str.Replace(" ", "-");
            return str;
        }

        //using System.Text

        private string RemoveAccent(string txt)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
