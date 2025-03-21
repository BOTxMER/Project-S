using Microsoft.EntityFrameworkCore;
using VapeApp.Data;
using VapeApp.Models;
using VapeApp.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VapeDbContext>(options =>
    options.UseSqlite("Data Source=vape.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/vape", async (CreateVapeDTO dto, VapeDbContext dbContext) =>
{
    var vape = new Vape(dto.Name ?? "Неизвестный вейп", dto.Laste ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Vapes.Add(vape);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/vape/{vape.Id}", vape);
});

app.MapPut("/vape/{id}", async (UpdateVapeDTO dto, int id, VapeDbContext dbContext) =>
{
    var vape = await dbContext.Vapes.FindAsync(id);
    if (vape == null)
    {
        return Results.NotFound("Вейп не найден");
    }

    if (dto.Name != null) vape.Name = dto.Name;
    if (dto.Laste != null) vape.Laste = dto.Laste;
    if (dto.Price.HasValue) vape.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(vape);
});

app.MapDelete("/vape/{id}", async (int id, VapeDbContext dbContext) =>
{
    var vape = await dbContext.Vapes.FindAsync(id);
    if (vape == null)
    {
        return Results.NotFound("Вейп не найден");
    }

    dbContext.Vapes.Remove(vape);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/vape", async (VapeDbContext dbContext) =>
{
    var vapes = await dbContext.Vapes.ToListAsync();
    return Results.Ok(vapes);
});

app.Run();
