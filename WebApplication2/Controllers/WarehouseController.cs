using APBD6.Models;
using APBD6.Repositories;
namespace Task6.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


public class WarehouseController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductWarehouseRepository _productWarehouseRepository;

    public WarehouseController(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IWarehouseRepository warehouseRepository,
        IProductWarehouseRepository productWarehouseRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _warehouseRepository = warehouseRepository;
        _productWarehouseRepository = productWarehouseRepository;
    }

    [HttpPost("addProductToWarehouse")]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] AddProductToWarehouseDTO dto)
    {
        if (dto.Amount <= 0)
        {
            return BadRequest("The amount must be greater than zero.");
        }
        
        var product = await _productRepository.GetProductByIdAsync(dto.IdProduct);
        if (product == null)
        {
            return NotFound("Product not found.");
        }
        
        var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(dto.IdWarehouse);
        if (warehouse == null)
        {
            return NotFound("Warehouse not found.");
        }
        
        var order = await _orderRepository.GetOrderByIdProductAndAmountAsync(dto.IdProduct, dto.Amount);
        if (order == null || order.CreatedAt >= dto.CreatedAt)
        {
            return BadRequest("No valid order found for the given product and amount.");
        }
        
        bool exists = await _productWarehouseRepository.CheckProductWarehouseExistenceAsync(order.IdOrder);
        if (exists)
        {
            return BadRequest("This order has already been fulfilled.");
        }
        
        bool orderUpdated = await _orderRepository.UpdateOrderFulfillmentDateAsync(order.IdOrder, dto.CreatedAt);
        if (!orderUpdated)
        {
            return StatusCode(500, "Failed to update the order fulfillment status.");
        }
        
        var productWarehouse = new ProductWarehouse
        {
            IdWarehouse = dto.IdWarehouse,
            IdProduct = dto.IdProduct,
            IdOrder = order.IdOrder,
            Amount = dto.Amount,
            Price = product.Price * dto.Amount,
            CreatedAt = dto.CreatedAt
        };

        bool added = await _productWarehouseRepository.AddProductToWarehouseAsync(productWarehouse);
        if (!added)
        {
            return StatusCode(500, "Failed to add the product to the warehouse.");
        }

        return Ok(new { Message = "Product added to warehouse successfully.", ProductWarehouseId = productWarehouse.IdProductWarehouse });
    }
}