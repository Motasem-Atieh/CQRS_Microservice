using CQRS_Microservice.Dto;
using CQRS_Microservice.Models;
using CQRS_Microservice.ProductCommand;
using CQRS_Microservice.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;

namespace CQRS_Microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ProductController> _logger;
        private readonly IRabbitMQService _rabbitMQService;

        public ProductController(IMediator mediator, IDistributedCache cache, ILogger<ProductController> logger, IRabbitMQService rabbitMQService)
        {
            _mediator = mediator;
            _cache = cache;
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        // Get all products (cached)
        [Authorize(Policy = "CanReadProduct")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "ProductList";
            var cachedProducts = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                var products = JsonSerializer.Deserialize<IEnumerable<ProductDto>>(cachedProducts);
                _logger.LogInformation($"Getting products from cache with key: {cacheKey}");
                return Ok(products);
            }

            var productsFromDb = await _mediator.Send(new GetProductQuery());
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(productsFromDb), cacheOptions);
            _logger.LogInformation($"Getting products from cache with key: {cacheKey}");
            return Ok(productsFromDb);
        }


        // Get a product by ID (cached)
        [Authorize(Policy = "CanReadProduct")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cacheKey = $"Product_{id}";
            var cachedProduct = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                var product = JsonSerializer.Deserialize<ProductDto>(cachedProduct);
                return Ok(product);
            }

            var productFromDb = await _mediator.Send(new GetProductByIdQuery(id));
            if (productFromDb == null)
                return NotFound();

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(productFromDb), cacheOptions);
            return Ok(productFromDb);
        }

        // Create a new product
        [Authorize(Policy = "CanCreateProduct")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
           

            _rabbitMQService.SendingMessage(command);

            

            return Ok();
            //###############


            //var productId = await _mediator.Send(command);

           
            //await _cache.RemoveAsync("ProductList");

           // return CreatedAtAction(nameof(Get), new { id = productId }, productId);
        }

        // Update a product
        [Authorize(Policy = "CanUpdateProduct")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound();

            await _cache.RemoveAsync("ProductList");
            await _cache.RemoveAsync($"Product_{id}");

            return NoContent();
        }

        // Delete a product
        [Authorize(Policy = "CanDeleteProduct")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id });
            if (!result)
                return NotFound();

            await _cache.RemoveAsync("ProductList");
            await _cache.RemoveAsync($"Product_{id}");

            return NoContent();
        }
    } 
}
