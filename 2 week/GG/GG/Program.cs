using Microsoft.EntityFrameworkCore;
using GameApp.Data;
using GameApp.Models;
using GameApp.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite("Data Source=game.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/game", async (CreateGameDTO dto, GameDbContext dbContext) =>
{
    var game = new Game(dto.Name ?? "Неизвестная игра", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Games.Add(game);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/game/{game.Id}", game);
});

app.MapPut("/game/{id}", async (UpdateGameDTO dto, int id, GameDbContext dbContext) =>
{
    var game = await dbContext.Games.FindAsync(id);
    if (game == null)
    {
        return Results.NotFound("Игра не найден");
    }

    if (dto.Name != null) game.Name = dto.Name;
    if (dto.Description != null) game.Description = dto.Description;
    if (dto.Price.HasValue) game.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(game);
});

app.MapDelete("/game/{id}", async (int id, GameDbContext dbContext) =>
{
    var game = await dbContext.Games.FindAsync(id);
    if (game == null)
    {
        return Results.NotFound("Игра не найден");
    }

    dbContext.Games.Remove(game);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/game", async (GameDbContext dbContext) =>
{
    var games = await dbContext.Games.ToListAsync();
    return Results.Ok(games);
});

app.Run();
