using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using APBD6.Models;

namespace Task6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly string connectionStr;

        public WarehouseController(IConfiguration configuration)
        {
            connectionStr = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("RegisterProduct")]
        public IActionResult ProcessProductInWarehouse([FromBody] ProductWarehouseRequest prodRequest)
        {
            if (prodRequest.Amount <= 0)
            {
                return BadRequest("Quantity must be positive");
            }

            using (var conn = new SqlConnection(connectionStr))
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                try
                {
                    if (!CheckIfProductExists(conn, trans, prodRequest.ProductId))
                    {
                        trans.Rollback();
                        return NotFound("Product not found");
                    }

                    if (!CheckIfWarehouseExists(conn, trans, prodRequest.WarehouseId))
                    {
                        trans.Rollback();
                        return NotFound("Warehouse not found");
                    }

                    int orderId = ProcessOrder(conn, trans, prodRequest);
                    if (orderId == -1)
                    {
                        trans.Rollback();
                        return BadRequest("Order wasn't found");
                    }

                    decimal price = GetProductPrice(conn, trans, prodRequest.ProductId);
                    if (price == -1)
                    {
                        trans.Rollback();
                        return BadRequest("Product price wasn't found");
                    }

                    decimal totalCost = price * prodRequest.Amount;
                    int insertedId = UpdateOrderAndInsertProductWarehouse(conn, trans, orderId, prodRequest, totalCost);

                    trans.Commit();
                    return Ok($"Record inserted with ID: {insertedId}");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }

        private bool CheckIfProductExists(SqlConnection conn, SqlTransaction trans, int productId)
        {
            string query = "SELECT 1 FROM [Product] WHERE IdProduct = @IdProduct";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@IdProduct", productId);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool CheckIfWarehouseExists(SqlConnection conn, SqlTransaction trans, int warehouseId)
        {
            string query = "SELECT 1 FROM [Warehouse] WHERE IdWarehouse = @IdWarehouse";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@IdWarehouse", warehouseId);
                return cmd.ExecuteScalar() != null;
            }
        }

        private int ProcessOrder(SqlConnection conn, SqlTransaction trans, ProductWarehouseRequest prodRequest)
        {
            string query = @"SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount 
                             AND CreatedAt < @CreatedAt AND IdOrder NOT IN (SELECT IdOrder FROM [Product_Warehouse])";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                cmd.Parameters.AddWithValue("@Amount", prodRequest.Amount);
                cmd.Parameters.AddWithValue("@CreatedAt", prodRequest.CreatedAt);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return -1;

                    return reader.GetInt32(0);
                }
            }
        }

        private decimal GetProductPrice(SqlConnection conn, SqlTransaction trans, int productId)
        {
            string query = "SELECT Price FROM [Product] WHERE IdProduct = @IdProduct";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@IdProduct", productId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return -1;

                    return reader.GetDecimal(0);
                }
            }
        }

        private int UpdateOrderAndInsertProductWarehouse(SqlConnection conn, SqlTransaction trans, int orderId, ProductWarehouseRequest prodRequest, decimal totalCost)
        {
            string updateQuery = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder";
            using (var updateCmd = new SqlCommand(updateQuery, conn, trans))
            {
                updateCmd.Parameters.AddWithValue("@IdOrder", orderId);
                updateCmd.ExecuteNonQuery();
            }

            string insertQuery = @"INSERT INTO [Product_Warehouse] (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt)
                                   VALUES (@IdProduct, @IdWarehouse, @IdOrder, @Amount, @Price, GETDATE()); SELECT SCOPE_IDENTITY();";
            using (var insertCmd = new SqlCommand(insertQuery, conn, trans))
            {
                insertCmd.Parameters.AddWithValue("@IdProduct", prodRequest.ProductId);
                insertCmd.Parameters.AddWithValue("@IdWarehouse", prodRequest.WarehouseId);
                insertCmd.Parameters.AddWithValue("@IdOrder", orderId);
                insertCmd.Parameters.AddWithValue("@Amount", prodRequest.Amount);
                insertCmd.Parameters.AddWithValue("@Price", totalCost);

                return Convert.ToInt32(insertCmd.ExecuteScalar());
            }
        }
    }
}
