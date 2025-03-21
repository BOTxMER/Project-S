using Microsoft.EntityFrameworkCore;
using WaterApp.Data;
using WaterApp.Models;
using WaterApp.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WaterDbContext>(options =>
    options.UseSqlite("Data Source=water.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/water", async (CreatePbDTO dto, WaterDbContext dbContext) =>
{
    var water = new Water(dto.Name ?? "Неизвестная вода", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Waters.Add(water);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/water/{water.Id}", water);
});

app.MapPut("/water/{id}", async (UpdateWaterDTO dto, int id, WaterDbContext dbContext) =>
{
    var water = await dbContext.Waters.FindAsync(id);
    if (water == null)
    {
        return Results.NotFound("Вода не найден");
    }

    if (dto.Name != null) water.Name = dto.Name;
    if (dto.Description != null) water.Description = dto.Description;
    if (dto.Price.HasValue) water.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(water);
});

app.MapDelete("/water/{id}", async (int id, WaterDbContext dbContext) =>
{
    var water = await dbContext.Waters.FindAsync(id);
    if (water == null)
    {
        return Results.NotFound("Вода не найден");
    }

    dbContext.Waters.Remove(water);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/water", async (WaterDbContext dbContext) =>
{
    var waters = await dbContext.Waters.ToListAsync();
    return Results.Ok(waters);
});

app.Run();
