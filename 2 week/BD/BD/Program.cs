using Microsoft.EntityFrameworkCore;
using TeaApp.Data;
using TeaApp.Models;
using TeaApp.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TeaDbContext>(options =>
    options.UseSqlite("Data Source=tea.db")); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/tea", async (CreateTeaDTO dto, TeaDbContext dbContext) =>
{
    var tea = new Tea(dto.Name ?? "Неизвестный чай", dto.Description ?? "Описание не предоставлено", dto.Price ?? 0);
    dbContext.Teas.Add(tea);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/tea/{tea.Id}", tea);
});

app.MapPut("/tea/{id}", async (UpdateTeaDTO dto, int id, TeaDbContext dbContext) =>
{
    var tea = await dbContext.Teas.FindAsync(id);
    if (tea == null)
    {
        return Results.NotFound("Чай не найден");
    }

    if (dto.Name != null) tea.Name = dto.Name;
    if (dto.Description != null) tea.Description = dto.Description;
    if (dto.Price.HasValue) tea.Price = dto.Price.Value;

    await dbContext.SaveChangesAsync();
    return Results.Ok(tea);
});

app.MapDelete("/tea/{id}", async (int id, TeaDbContext dbContext) =>
{
    var tea = await dbContext.Teas.FindAsync(id);
    if (tea == null)
    {
        return Results.NotFound("Чай не найден");
    }

    dbContext.Teas.Remove(tea);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/tea", async (TeaDbContext dbContext) =>
{
    var teas = await dbContext.Teas.ToListAsync();
    return Results.Ok(teas);
});

app.Run();
