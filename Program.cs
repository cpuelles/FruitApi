using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
builder.Services.AddDbContext<FruitDb>(opt => opt.UseInMemoryDatabase("FruitList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Fruit API",
        Description = "API for managing a list of fruit and their stock status.",
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FruitDb>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/fruitlist",  async (FruitDb db) =>
    await db.Fruits.Include(f => f.ColorNavigation).ToListAsync())
    .WithTags("Get all fruit"); 

app.MapGet("/fruitlist/instock", async (FruitDb db) =>
    await db.Fruits.Where(t => t.Instock).ToListAsync())
    .WithTags("Get all fruit that is in stock");

app.MapGet("/fruitlist/{id}", async (int id, FruitDb db) =>
    await db.Fruits.FindAsync(id)
        is Fruit fruit
            ? Results.Ok(fruit)
            : Results.NotFound())
    .WithTags("Get fruit by Id");

app.MapPost("/fruitlist", async (Fruit fruit, FruitDb db) =>
{
    db.Fruits.Add(fruit);
    await db.SaveChangesAsync();

    return Results.Created($"/fruitlist/{fruit.Id}", fruit);
})
    .WithTags("Add fruit to list");

app.MapPut("/fruitlist/{id}", async (int id, Fruit inputFruit, FruitDb db) =>
{
    var fruit = await db.Fruits.FindAsync(id);

    if (fruit is null) return Results.NotFound();

    fruit.Name = inputFruit.Name;
    fruit.Instock = inputFruit.Instock;
    fruit.ColorId = inputFruit.ColorId;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
    .WithTags("Update fruit by Id");

app.MapDelete("/fruitlist/{id}", async (int id, FruitDb db) =>
{
    if (await db.Fruits.FindAsync(id) is Fruit fruit)
    {
        db.Fruits.Remove(fruit);
        await db.SaveChangesAsync();
        return Results.Ok(fruit);
    }

    return Results.NotFound();
})
    .WithTags("Delete fruit by Id");

/* ----------------------------- CUSTOM ATTRIBUTES ------------------------------------ */

app.MapGet("/fruitlist/color",  async (FruitDb db) =>
    await db.Colors.ToListAsync())
    .WithTags("Get all colors"); 

app.MapGet("/fruitlist/color/{id}", async (int id, FruitDb db) =>
    await db.Colors.FindAsync(id)
        is Color color
            ? Results.Ok(color)
            : Results.NotFound())
    .WithTags("Get color by Id");

app.MapPost("/fruitlist/color", async (Color color, FruitDb db) =>
{
    db.Colors.Add(color);
    await db.SaveChangesAsync();

    return Results.Created($"/fruitlist/color/{color.Id}", color);
})
    .WithTags("Add color to list");

app.MapPut("/fruitlist/color/{id}", async (int id, Color inputColor, FruitDb db) =>
{
    var color = await db.Colors.FindAsync(id);

    if (color is null) return Results.NotFound();

    color.Name = inputColor.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
    .WithTags("Update color by Id");

app.MapDelete("/fruitlist/color/{id}", async (int id, FruitDb db) =>
{
    if (await db.Colors.FindAsync(id) is Color color)
    {
        db.Colors.Remove(color);
        await db.SaveChangesAsync();
        return Results.Ok(color);
    }

    return Results.NotFound();
})
    .WithTags("Delete color by Id");

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

app.Run();