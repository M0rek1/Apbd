namespace task5.Properties;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AnimalController : ControllerBase
{
    private readonly AnimalsContext _context;

    public AnimalController(AnimalsContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAnimal([FromQuery] string orderBy = "name")
    {
        IQueryable<Animal> query = _context.Animal;

        switch (orderBy.ToLower())
        {
            case "name":
                query = query.OrderBy(a => a.Name);
                break;
            case "description":
                query = query.OrderBy(a => a.Description);
                break;
            case "category":
                query = query.OrderBy(a => a.Category);
                break;
            case "area":
                query = query.OrderBy(a => a.Area);
                break;
            default:
                return BadRequest("Invalid orderBy parameter");
        }

        return await query.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
    {
        _context.Animal.Add(animal);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAnimal), new { id = animal.IdAnimal }, animal);
    }


   
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnimal(int id, Animal animal)
    {
        if (id != animal.IdAnimal)
        {
            return BadRequest();
        }

        _context.Entry(animal).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Animal.Any(e => e.IdAnimal == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
        var animal = await _context.Animal.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }

        _context.Animal.Remove(animal);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}