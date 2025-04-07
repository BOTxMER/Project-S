using Microsoft.EntityFrameworkCore;
using GameApp.Data;
using GameApp.Models;
using GameApp.DTOs;
using GameApp.Data;

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
    game.Name = dto.Name;
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

// Search
app.MapGet("/game/search", async (string query, GameDbContext dbContext) =>
{
    var results = await dbContext.Games
        .Where(c => c.Name.Contains(query) || c.Description.Contains(query))
        .ToListAsync();

    return Results.Ok(results);
});

// Filt
app.MapGet("/game/filter", async (string? name, double? minPrice, double? maxPrice, GameDbContext dbContext) =>
{
    IQueryable<Game> games = dbContext.Games;

    if (!string.IsNullOrWhiteSpace(name))
        games = games.Where(c => c.Name.Contains(name));

    if (minPrice.HasValue)
        games = games.Where(c => c.Price >= minPrice.Value);

    if (maxPrice.HasValue)
        games = games.Where(c => c.Price <= maxPrice.Value);

    return Results.Ok(await games.ToListAsync());
});

// Sort
app.MapGet("/game/sort", async (string orderBy, GameDbContext dbContext) =>
{
    IQueryable<Game> query = dbContext.Games;

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
app.MapGet("/game/page", async (int? page, int? pageSize, GameDbContext dbContext) =>
{
    int p = page.GetValueOrDefault(1);
    int ps = pageSize.GetValueOrDefault(10);

    if (p <= 0) p = 1;
    if (ps <= 0 || ps > 100) ps = 10;

    var total = await dbContext.Games.CountAsync();

    var games = await dbContext.Games
        .Skip((p - 1) * ps)
        .Take(ps)
        .ToListAsync();

    return Results.Ok(new
    {
        total,
        page = p,
        pageSize = ps,
        totalPages = (int)Math.Ceiling(total / (double)ps),
        data = games
    });
});



app.MapGet("/game/random", async (GameDbContext dbContext) =>
{
    var total = await dbContext.Games.CountAsync();
    if (total == 0)
        return Results.NotFound("В базе нет игр");

    var random = new Random();
    int skip = random.Next(total);

    var randomCar = await dbContext.Games.Skip(skip).FirstOrDefaultAsync();
    return Results.Ok(randomCar);
});

app.MapGet("/game/most-expensive", async (GameDbContext dbContext) =>
{
    var mostExpensive = await dbContext.Games
        .OrderByDescending(g => g.Price)
        .FirstOrDefaultAsync();

    if (mostExpensive == null)
        return Results.NotFound("Игры не найдены");

    return Results.Ok(mostExpensive);
});

app.Run();
