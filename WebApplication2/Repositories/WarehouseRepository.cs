using Microsoft.Data.SqlClient;
using APBD6.Models;

namespace APBD6.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<Warehouse> GetWarehouseByIdAsync(int idWarehouse)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("SELECT * FROM [Warehouse] WHERE IdWarehouse = @idWarehouse", connection))
            {
                command.Parameters.AddWithValue("@idWarehouse", idWarehouse);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Warehouse
                        {
                            IdWarehouse = (int)reader["IdWarehouse"],
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                        };
                    }
                }
            }
        }
        return null;
    }
}