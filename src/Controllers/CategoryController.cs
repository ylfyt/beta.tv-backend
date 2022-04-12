using src.Data;
using Microsoft.AspNetCore.Mvc;
using src.Dtos;
using src.Dtos.category;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IResponseGetter<DataCategory> _responseGetterCategory;
        private readonly IResponseGetter<DataCategories> _responseGetterCategories;
        public CategoryController(DataContext context, IResponseGetter<DataCategory> responseGetterCattegory, IResponseGetter<DataCategories> responseGetterCategories)
        {
            _context = context;
            _responseGetterCategory = responseGetterCattegory;
            _responseGetterCategories = responseGetterCategories;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<DataCategories>>> GET()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_responseGetterCategories.Success(new DataCategories
            {
                categories = categories
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DataCategory>>> GET(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(_responseGetterCategory.Error());
            }

            return Ok(_responseGetterCategory.Success(new DataCategory
            {
                category = category
            }));
        }
    }
}
