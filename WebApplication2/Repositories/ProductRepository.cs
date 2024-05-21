using APBD6.Models;

namespace APBD6.Repositories;

using Microsoft.Data.SqlClient;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<Product> GetProductByIdAsync(int idProduct)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("SELECT * FROM [Product] WHERE IdProduct = @idProduct", connection))
            {
                command.Parameters.AddWithValue("@idProduct", idProduct);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Product
                        {
                            IdProduct = (int)reader["IdProduct"],
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Price = (decimal)reader["Price"]
                        };
                    }
                }
            }
        }
        return null;
    }
}