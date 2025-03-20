using Microsoft.EntityFrameworkCore;
using PbApp.Data;
using PbApp.Models;
using PbApp.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PbDbContext>(options =>
    options.UseSqlite("Data Source=powerbank.db")); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/powerbank", async (CreatePbDTO dto, PbDbContext dbContext) =>
{
    var powerbank = new Powerbank(dto.Name ?? "Неизвестный повербанк", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Powerbanks.Add(powerbank);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/powerbank/{powerbank.Id}", powerbank);
});

app.MapPut("/powerbank/{id}", async (UpdatePbDTO dto, int id, PbDbContext dbContext) =>
{
    var powerbank = await dbContext.Powerbanks.FindAsync(id);
    if (powerbank == null)
    {
        return Results.NotFound("Повербанк не найден");
    }

    if (dto.Name != null) powerbank.Name = dto.Name;
    if (dto.Description != null) powerbank.Description = dto.Description;
    if (dto.Price.HasValue) powerbank.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(powerbank);
});

app.MapDelete("/powerbank/{id}", async (int id, PbDbContext dbContext) =>
{
    var powerbank = await dbContext.Powerbanks.FindAsync(id);
    if (powerbank == null)
    {
        return Results.NotFound("Повербанк не найден");
    }

    dbContext.Powerbanks.Remove(powerbank);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/powerbank", async (PbDbContext dbContext) =>
{
    var powerbanks = await dbContext.Powerbanks.ToListAsync();
    return Results.Ok(powerbanks);
});

app.Run();
