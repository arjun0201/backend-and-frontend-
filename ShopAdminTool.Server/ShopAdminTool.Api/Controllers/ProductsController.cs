using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopAdminTool.Api.Resources;
using ShopAdminTool.Application;
using ShopAdminTool.Core;

namespace ShopAdminTool.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get products
    /// </summary>
    /// <returns><see cref="Task{IActionResult}"/>.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<Product>>> Get() 
        => Ok(await _mediator.Send(new GetProductsQuery()));

    /// <summary>
    /// Get product by Id
    /// </summary>
    /// <param name="id">Product id</param>
    /// <returns><see cref="Task{IActionResult}"/>.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(string id) 
        => Ok(await _mediator.Send(new GetProductQuery(id)));

    /// <summary>
    /// Create product
    /// </summary>
    /// <param name="product">Product info</param>
    /// <returns><see cref="Task{IActionResult}"/>.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody]ProductDto product)
    {
        await _mediator.Send(new CreateProductCommand(product));
        return CreatedAtAction(nameof(GetById), new { product.Id }, product);
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="id">Product id</param>
    /// <param name="product">Product info</param>
    /// <returns><see cref="Task{IActionResult}"/>.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody]ProductDto product)
    {
        if(id != product.Id)
        {
            return BadRequest(ErrorMessages.IdsAreNotEqualExceptionMessage);
        }
        await _mediator.Send(new UpdateProductCommand(id, product));
        return Ok();
    }

    /// <summary>
    /// Delete product
    /// </summary>
    /// <param name="id">Product id</param>
    /// <returns><see cref="Task{IActionResult}"/>.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id) 
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return Ok();
    }
}
