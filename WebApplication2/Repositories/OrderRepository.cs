using APBD6.Models;

namespace APBD6.Repositories;

using Microsoft.Data.SqlClient;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<Order> GetOrderByIdProductAndAmountAsync(int idProduct, int amount)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("SELECT * FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount", connection))
            {
                command.Parameters.AddWithValue("@idProduct", idProduct);
                command.Parameters.AddWithValue("@amount", amount);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Order
                        {
                            IdOrder = (int)reader["IdOrder"],
                            IdProduct = (int)reader["IdProduct"],
                            Amount = (int)reader["Amount"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            FulfilledAt = reader.IsDBNull(reader.GetOrdinal("FulfilledAt")) ? (DateTime?)null : (DateTime)reader["FulfilledAt"]
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<bool> UpdateOrderFulfillmentDateAsync(int idOrder, DateTime fulfilledAt)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "UPDATE [Order] SET FulfilledAt = @fulfilledAt WHERE IdOrder = @idOrder";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@fulfilledAt", fulfilledAt);
                command.Parameters.AddWithValue("@idOrder", idOrder);

                int affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
        }
    }

}