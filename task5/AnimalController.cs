using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using task5.Properties;

namespace Lesson6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalController : ControllerBase
{
    private readonly string _dbConnectionString;

    public AnimalController(IConfiguration config)
    {
        _dbConnectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> FetchAnimals([FromQuery] string sortBy = "name")
    {
        var animalList = new List<Animal>();
        var sqlQuery = $"SELECT * FROM Animal ORDER BY {sortBy}";

        using (var dbConnection = new SqlConnection(_dbConnectionString))
        {
            using (var sqlCommand = new SqlCommand(sqlQuery, dbConnection))
            {
                await dbConnection.OpenAsync();
                using (var dataReader = await sqlCommand.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        animalList.Add(new Animal
                        {
                            IdAnimal = dataReader.GetInt32("IdAnimal"),
                            Name = dataReader.GetString("Name"),
                            Description = dataReader.IsDBNull("Description") ? null : dataReader.GetString("Description"),
                            Category = dataReader.GetString("Category"),
                            Area = dataReader.GetString("Area")
                        });
                    }
                }
            }
        }

        return Ok(animalList);
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> CreateAnimal([FromBody] Animal newAnimal)
    {
        var insertQuery = "INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area); " +
                          "SELECT CAST(SCOPE_IDENTITY() as int);";

        using (var dbConnection = new SqlConnection(_dbConnectionString))
        {
            using (var sqlCommand = new SqlCommand(insertQuery, dbConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Name", newAnimal.Name);
                sqlCommand.Parameters.AddWithValue("@Description", newAnimal.Description ?? (object)DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Category", newAnimal.Category);
                sqlCommand.Parameters.AddWithValue("@Area", newAnimal.Area);

                await dbConnection.OpenAsync();
                newAnimal.IdAnimal = (int)await sqlCommand.ExecuteScalarAsync();
            }
        }

        return CreatedAtAction(nameof(FetchAnimals), new { id = newAnimal.IdAnimal }, newAnimal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
    {
        if (id != updatedAnimal.IdAnimal)
            return BadRequest();

        var updateQuery = "UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";

        using (var dbConnection = new SqlConnection(_dbConnectionString))
        {
            using (var sqlCommand = new SqlCommand(updateQuery, dbConnection))
            {
                sqlCommand.Parameters.AddWithValue("@IdAnimal", updatedAnimal.IdAnimal);
                sqlCommand.Parameters.AddWithValue("@Name", updatedAnimal.Name);
                sqlCommand.Parameters.AddWithValue("@Description", updatedAnimal.Description ?? (object)DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Category", updatedAnimal.Category);
                sqlCommand.Parameters.AddWithValue("@Area", updatedAnimal.Area);

                await dbConnection.OpenAsync();
                var updateResult = await sqlCommand.ExecuteNonQueryAsync();

                if (updateResult == 0)
                    return NotFound();
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveAnimal(int id)
    {
        var deleteQuery = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";

        using (var dbConnection = new SqlConnection(_dbConnectionString))
        {
            using (var sqlCommand = new SqlCommand(deleteQuery, dbConnection))
            {
                sqlCommand.Parameters.AddWithValue("@IdAnimal", id);

                await dbConnection.OpenAsync();
                var deleteResult = await sqlCommand.ExecuteNonQueryAsync();

                if (deleteResult == 0)
                    return NotFound();
            }
        }

        return NoContent();
    }
}
