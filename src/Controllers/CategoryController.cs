using src.Data;
using Microsoft.AspNetCore.Mvc;
using src.Dtos.category;

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
        public async Task<ActionResult<Many>> GET()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_responseGetterMany.Success(new DataCategories
            {
                categories = categories
            }));
        }

        [HttpGet("{id}")]
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
    }
}
