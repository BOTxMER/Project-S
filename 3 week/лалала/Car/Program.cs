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

app.MapGet("/car", async (
    string? sort,
    string? model,
    double? minPrice,
    double? maxPrice,
    string? search,
    int page = 1,
    int pageSize = 10,
    CarDbContext dbContext) =>
{
    if (page <= 0) page = 1;
    if (pageSize <= 0 || pageSize > 100) pageSize = 10;

    IQueryable<Car> query = dbContext.Cars;

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(c =>
            c.Model.Contains(search) || c.Description.Contains(search));
    }

    // Filters
    if (!string.IsNullOrWhiteSpace(model))
    {
        query = query.Where(c => c.Model.Contains(model));
    }

    if (minPrice.HasValue)
    {
        query = query.Where(c => c.Price >= minPrice.Value);
    }

    if (maxPrice.HasValue)
    {
        query = query.Where(c => c.Price <= maxPrice.Value);
    }

    // Sort
    query = sort switch
    {
        "price_desc" => query.OrderByDescending(c => c.Price),
        "created_asc" => query.OrderBy(c => c.CreatedAt),
        "created_desc" => query.OrderByDescending(c => c.CreatedAt),
        _ => query.OrderBy(c => c.Price) 
    };

    // Pagination
    var totalCount = await query.CountAsync();
    var cars = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var result = new
    {
        totalCount,
        page,
        pageSize,
        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        data = cars
    };

    return Results.Ok(result);
});


app.Run();
