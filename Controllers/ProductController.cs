using if3250_2022_01_buletin_backend.Data;
using if3250_2022_01_buletin_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace if3250_2022_01_buletin_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GET()
        {
            return await _context.Products.ToListAsync();
        }
    }
}
