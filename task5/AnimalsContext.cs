namespace task5.Properties;


using Microsoft.EntityFrameworkCore;

public class AnimalsContext : DbContext
{
    public AnimalsContext(DbContextOptions<AnimalsContext> options) : base(options)
    {
    }

    public DbSet<Animal> Animal { get; set; }
}