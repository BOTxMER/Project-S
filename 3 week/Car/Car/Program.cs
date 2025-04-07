using Microsoft.EntityFrameworkCore;
using CarApp.Data;
using CarApp.Models;
using CarApp.DTOs;
using CarApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CarDbContext>(options =>
    options.UseSqlite("Data Source=game.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/car", async (CreateCarDTO dto, CarDbContext dbContext) =>
{
    var car = new Car(dto.Model ?? "Неизвестная машина", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Cars.Add(car);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/car/{car.Id}", car);
});

app.MapPut("/car/{id}", async (UpdateCarDTO dto, int id, CarDbContext dbContext) =>
{
    var car = await dbContext.Cars.FindAsync(id);
    if (car == null)
    {
        return Results.NotFound("Машина не найден");
    }

    if (dto.Model != null) car.Model = dto.Model;
    if (dto.Description != null) car.Description = dto.Description;
    if (dto.Price.HasValue) car.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(car);
});

app.MapDelete("/car/{id}", async (int id, CarDbContext dbContext) =>
{
    var car = await dbContext.Cars.FindAsync(id);
    if (car == null)
    {
        return Results.NotFound("Машина не найден");
    }

    dbContext.Cars.Remove(car);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/car", async (CarDbContext dbContext) =>
{
    var cars = await dbContext.Cars.ToListAsync();
    return Results.Ok(cars);
});

// Search
app.MapGet("/car/search", async (string query, CarDbContext dbContext) =>
{
    var results = await dbContext.Cars
        .Where(c => c.Model.Contains(query) || c.Description.Contains(query))
        .ToListAsync();

    return Results.Ok(results);
});

// Filt
app.MapGet("/car/filter", async (string? model, double? minPrice, double? maxPrice, CarDbContext dbContext) =>
{
    IQueryable<Car> cars = dbContext.Cars;

    if (!string.IsNullOrWhiteSpace(model))
        cars = cars.Where(c => c.Model.Contains(model));

    if (minPrice.HasValue)
        cars = cars.Where(c => c.Price >= minPrice.Value);

    if (maxPrice.HasValue)
        cars = cars.Where(c => c.Price <= maxPrice.Value);

    return Results.Ok(await cars.ToListAsync());
});

// Sort
app.MapGet("/car/sort", async (string orderBy, CarDbContext dbContext) =>
{
    IQueryable<Car> query = dbContext.Cars;

    query = orderBy switch
    {
        "price_desc" => query.OrderByDescending(c => c.Price),
        "created_asc" => query.OrderBy(c => c.CreatedAt),
        "created_desc" => query.OrderByDescending(c => c.CreatedAt),
        _ => query.OrderBy(c => c.Price)
    };

    return Results.Ok(await query.ToListAsync());
});

// Pagination
app.MapGet("/car/page", async (int? page, int? pageSize, CarDbContext dbContext) =>
{
    int p = page.GetValueOrDefault(1);
    int ps = pageSize.GetValueOrDefault(10);

    if (p <= 0) p = 1;
    if (ps <= 0 || ps > 100) ps = 10;

    var total = await dbContext.Cars.CountAsync();

    var cars = await dbContext.Cars
        .Skip((p - 1) * ps)
        .Take(ps)
        .ToListAsync();

    return Results.Ok(new
    {
        total,
        page = p,
        pageSize = ps,
        totalPages = (int)Math.Ceiling(total / (double)ps),
        data = cars
    });
});



app.MapGet("/car/random", async (CarDbContext dbContext) =>
{
    var total = await dbContext.Cars.CountAsync();
    if (total == 0)
        return Results.NotFound("В базе нет машин");

    var random = new Random();
    int skip = random.Next(total);

    var randomCar = await dbContext.Cars.Skip(skip).FirstOrDefaultAsync();
    return Results.Ok(randomCar);
});

app.Run();
