using ApiSample.Controllers.Request;
using ApiSample.Persistence.EFCore;
using ApiSample.Persistence.Entities;
using ApiSample.Persistence.Queries;
using ApiSample.Persistence.Queries.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSample.Controllers
{
    [ApiController]
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        readonly IServiceProvider _services;
        public ProductController(IServiceProvider services)
        {
           _services = services;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var dbcontext = _services.GetRequiredService<SampleContext>();

            dbcontext.Add(new Product
            {
                Name = request.Name,
                Price = request.Price,
                Available = request.Available,
                CategoryId = request.CategoryId,
                Description = request.Description,                
            });

            await dbcontext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery] SearchProductFilters request)
        {
            var queries = _services.GetRequiredService<IProductQueries>();

            var response = await queries.Search(request);

            return Ok(response);
        }
    }
}
