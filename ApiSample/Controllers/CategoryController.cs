using ApiSample.Controllers.Request;
using ApiSample.Persistence.EFCore;
using ApiSample.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSample.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        readonly IServiceProvider _services;
        public CategoryController(IServiceProvider services)
        {
           _services = services;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var dbcontext = _services.GetRequiredService<SampleContext>();

            dbcontext.Add(new Category
            {
                Name = request.Name,
            });

            await dbcontext.SaveChangesAsync();

            return Ok();
        }
    }
}
