using task5.Properties;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AnimalsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<AnimalsContext>();

    try
    {
        logger.LogInformation("Testing database connection...");
        context.Database.OpenConnection();
        logger.LogInformation("Database connection successful.");
        context.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to establish database connection.");
    }
}

app.Run();