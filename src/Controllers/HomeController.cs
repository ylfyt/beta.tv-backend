using src.Models;
using Microsoft.AspNetCore.Mvc;

namespace src.Controllers
{
    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
    }

    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult<String> Get()
        {
            return "Hello, World!";
        }

        [HttpGet("person")]
        public ActionResult<Person> person()
        {
            var person = new Person
            {
                Name = "Budi",
                Age = 42
            };

            return person;
        }
    }
}