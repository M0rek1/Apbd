using Microsoft.Data.SqlClient;
using APBD6.Models;

namespace APBD6.Repositories;

public class ProductWarehouseRepository : IProductWarehouseRepository
{
    private readonly string _connectionString;

    public ProductWarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> AddProductToWarehouseAsync(ProductWarehouse productWarehouse)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string sql = "INSERT INTO [Product_Warehouse] (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
                command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
                command.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);
                command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
                command.Parameters.AddWithValue("@Price", productWarehouse.Price);
                command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

                int affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
        }
    }

    public async Task<bool> CheckProductWarehouseExistenceAsync(int idOrder)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM [Product_Warehouse] WHERE IdOrder = @idOrder", connection))
            {
                command.Parameters.AddWithValue("@idOrder", idOrder);

                int existenceCount = (int)await command.ExecuteScalarAsync();
                return existenceCount > 0;
            }
        }
    }
}