using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using APBD6.Models;

namespace Task6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly string dbConnection = "Server=localhost;Database=APBD;User Id=sa;Password=yourStrong(!)Password;";

        [HttpPost("RegisterProduct")]
        public IActionResult ProcessProductInWarehouse([FromBody] ProductWarehouseRequest prodRequest)
        {
            if (prodRequest.Amount <= 0)
            {
                return BadRequest("Quantity must be positive");
            }

            using (var conn = new SqlConnection(dbConnection))
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                try
                {
                    string productQuery = "SELECT 1 FROM Product WHERE IdProduct = @IdProduct";
                    var productCmd = new SqlCommand(productQuery, conn, trans);
                    productCmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                    if (productCmd.ExecuteScalar() == null)
                    {
                        trans.Rollback();
                        return NotFound("Product not found");
                    }

                    string warehouseQuery = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
                    var warehouseCmd = new SqlCommand(warehouseQuery, conn, trans);
                    warehouseCmd.Parameters.AddWithValue("@IdWarehouse", prodRequest.WarehouseId);
                    if (warehouseCmd.ExecuteScalar() == null)
                    {
                        trans.Rollback();
                        return NotFound("Warehouse not found");
                    }

                    string orderQuery = @"SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount 
                                          AND CreatedAt < @CreatedAt AND IdOrder NOT IN (SELECT IdOrder FROM Product_Warehouse)";
                    var orderCmd = new SqlCommand(orderQuery, conn, trans);
                    orderCmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                    orderCmd.Parameters.AddWithValue("@Amount", prodRequest.Amount);
                    orderCmd.Parameters.AddWithValue("@CreatedAt", prodRequest.CreatedAt);

                    int orderId;
                    using (var reader = orderCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            trans.Rollback();
                            return BadRequest("Order wasn't found");
                        }
                        orderId = reader.GetInt32(0);
                        reader.Close();
                    }

                    decimal price;
                    string priceQuery = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
                    var priceCmd = new SqlCommand(priceQuery, conn, trans);
                    priceCmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                    using (var priceReader = priceCmd.ExecuteReader())
                    {
                        if (!priceReader.Read())
                        {
                            trans.Rollback();
                            return BadRequest("Product price wasn't found");
                        }
                        price = priceReader.GetDecimal(0);
                        priceReader.Close();
                    }

                    decimal totalCost = price * prodRequest.Amount;

                    string updateOrderQuery = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder";
                    var updateCmd = new SqlCommand(updateOrderQuery, conn, trans);
                    updateCmd.Parameters.AddWithValue("@IdOrder", orderId);
                    updateCmd.ExecuteNonQuery();

                    string insertQuery = @"INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt)
                                           VALUES (@IdProduct, @IdWarehouse, @IdOrder, @Amount, @Price, GETDATE()); SELECT SCOPE_IDENTITY();";
                    var insertCmd = new SqlCommand(insertQuery, conn, trans);
                    insertCmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                    insertCmd.Parameters.AddWithValue("@IdWarehouse", prodRequest.WarehouseId);
                    insertCmd.Parameters.AddWithValue("@IdOrder", orderId);
                    insertCmd.Parameters.AddWithValue("@Amount", prodRequest.Amount);
                    insertCmd.Parameters.AddWithValue("@Price", totalCost);

                    int insertedId = Convert.ToInt32(insertCmd.ExecuteScalar());

                    trans.Commit();
                    return Ok(insertedId);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
