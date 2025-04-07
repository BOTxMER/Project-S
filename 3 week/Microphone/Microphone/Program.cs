using Microsoft.EntityFrameworkCore;
using MicApp.Data;
using MicApp.Models;
using MicApp.DTOs;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MicDbContext>(options =>
    options.UseSqlite("Data Source=mic.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/mic", async (CreateMicDTO dto, MicDbContext dbContext) =>
{
    var car = new Mic(dto.Model ?? "Неизвестная машина", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Mics.Add(car);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/car/{car.Id}", car);
});

app.MapPut("/mic/{id}", async (UpdateMicDTO dto, int id, MicDbContext dbContext) =>
{
    var mic = await dbContext.Mics.FindAsync(id);
    if (mic == null)
    {
        return Results.NotFound("Машина не найден");
    }

    if (dto.Model != null) mic.Model = dto.Model;
    if (dto.Description != null) mic.Description = dto.Description;
    if (dto.Price.HasValue) mic.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(mic);
});

app.MapDelete("/mic/{id}", async (int id, MicDbContext dbContext) =>
{
    var mic = await dbContext.Mics.FindAsync(id);
    if (mic == null)
    {
        return Results.NotFound("Машина не найден");
    }

    dbContext.Mics.Remove(mic);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/car", async (
    [FromServices] MicDbContext dbContext,
    string? sort,
    string? model,
    double? minPrice,
    double? maxPrice,
    string? search,
    int page = 1,
    int pageSize = 10) =>
{
    if (page <= 0) page = 1;
    if (pageSize <= 0 || pageSize > 100) pageSize = 10;

    IQueryable<Mic> query = dbContext.Mics;

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
    var mics = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var result = new
    {
        totalCount,
        page,
        pageSize,
        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        data = mics
    };

    return Results.Ok(result);
});

app.MapGet("/mic/stats", async ([FromServices] MicDbContext dbContext) =>
{
    if (!await dbContext.Mics.AnyAsync())
        return Results.NotFound("Нет микрофонов для анализа.");

    var stats = new
    {
        total = await dbContext.Mics.CountAsync(),
        minPrice = await dbContext.Mics.MinAsync(m => m.Price),
        maxPrice = await dbContext.Mics.MaxAsync(m => m.Price),
        avgPrice = Math.Round(await dbContext.Mics.AverageAsync(m => m.Price), 2)
    };

    return Results.Ok(stats);
});


app.Run();
