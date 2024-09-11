using AutoMapper;
using CQRS_Microservice.Dto;
using CQRS_Microservice.Helper;
using CQRS_Microservice.Models;
using CQRS_Microservice.ProductCommand;
using CQRS_Microservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRS_Microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(
            CacheService<ProductDto> _cacheService,
            DatabaseService<Product> _databaseService,
            IRabbitMQService _rabbitMQService,
            IMapper _mapper,
            ILogger<ProductController> _logger) : ControllerBase
    {
       
        [Authorize(Policy = PermissionHelper.CanReadProduct)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "ProductList";
            var cachedProducts = await _cacheService.GetCollectionFromCacheAsync(cacheKey);

            if (cachedProducts == null) //if no products in cache
            {
                var products = await _databaseService.GetAllAsync(); //get from db
                var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products); //map

                // Cache the products
                await _cacheService.SetCacheAsync(cacheKey, productsDto);
                cachedProducts = productsDto;
            }

            _logger.LogInformation($"Retrieved product list for cache key: {cacheKey}");
            return Ok(cachedProducts);
        }

        [Authorize(Policy = PermissionHelper.CanReadProduct)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cacheKey = $"Product_{id}";
            var cachedProduct = await _cacheService.GetFromCacheAsync(cacheKey); //get 1 product from cache

            if (cachedProduct == null) //if not in cache
            {
                var product = await _databaseService.GetByIdAsync(id); //get from db
                if (product == null) //if not in db
                    return NotFound();

                //if in db but not in cache, add to cache

                var productDto = _mapper.Map<ProductDto>(product); 
                await _cacheService.SetCacheAsync(cacheKey, productDto);
                cachedProduct = productDto;
            }

            _logger.LogInformation($"Retrieved product for cache key: {cacheKey}");
            return Ok(cachedProduct);
        }

        [Authorize(Policy = PermissionHelper.CanCreateProduct)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            _rabbitMQService.SendingMessage(command); //send a create request to queue

            _logger.LogInformation("Product creation command sent to RabbitMQ.");
            return Ok();
        }

        [Authorize(Policy = PermissionHelper.CanUpdateProduct)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            _rabbitMQService.SendingMessage(command); //send a update request to queue

            _logger.LogInformation($"Product update command sent to RabbitMQ for ID: {id}");
            return NoContent();
        }

        [Authorize(Policy = PermissionHelper.CanDeleteProduct)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteCommand = new DeleteProductCommand { Id = id };
            _rabbitMQService.SendingMessage(deleteCommand); //send a delete request to queue

            _logger.LogInformation($"Product delete command sent to RabbitMQ for ID: {id}");
            return NoContent();
        }
    }
}

