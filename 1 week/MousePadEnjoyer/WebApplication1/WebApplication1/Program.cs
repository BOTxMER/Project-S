using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

List<MousePad> mousePads = new List<MousePad>
{
    new MousePad("Коврик для мыши с подстветкой", 120),
    new MousePad("Игровой коврик", 99),
    new MousePad("Коврик с подставкой для запястья", 999)
};

string mousePadRest = "mousepad";

app.MapPost(mousePadRest, (CreateMousePadDTO dto) =>
{
    MousePad mousePad = new MousePad(dto.Name ?? "Коврик без названия", dto.Price ?? 0);
    mousePads.Add(mousePad);
    return Results.Created($"/{mousePadRest}/{mousePads.IndexOf(mousePad)}", mousePad);
});

app.MapPut($"{mousePadRest}/{{index}}", (UpdateMousePadDTO dto, int index) =>
{
    if (index < 0 || index >= mousePads.Count)
    {
        return Results.NotFound("Коврик не найден");
    }

    MousePad mousePad = mousePads[index];

    if (dto.Name != null) mousePad.Name = dto.Name;
    if (dto.Price.HasValue) mousePad.Price = dto.Price.Value;

    return Results.Ok(mousePad);
});

app.MapDelete($"{mousePadRest}/{{index}}", (int index) =>
{
    if (index < 0 || index >= mousePads.Count)
    {
        return Results.NotFound("Коврик не найден");
    }

    mousePads.RemoveAt(index);
    return Results.NoContent();
});

app.MapGet(mousePadRest, () =>
{
    return Results.Ok(mousePads);
});

app.Run();

class MousePad
{
    public string Name { get; set; }
    public double Price { get; set; }
    public DateTime CreatedDate { get; set; }

    public MousePad(string name, double price)
    {
        Name = name;
        Price = price;
        CreatedDate = DateTime.Now;
    }
}

record CreateMousePadDTO(string? Name, double? Price);

record UpdateMousePadDTO(string? Name, double? Price);
