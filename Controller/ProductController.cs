using CQRS_Microservice.ProductCommand;
using CQRS_Microservice.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CQRS_Microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = "CanReadProduct")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _mediator.Send(new GetProductQuery());
            return Ok(products);
        }

        [Authorize(Policy = "CanReadProduct")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [Authorize(Policy = "CanCreateProduct")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = productId }, productId);
        }

        [Authorize(Policy = "CanUpdateProduct")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "CanDeleteProduct")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id });
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
