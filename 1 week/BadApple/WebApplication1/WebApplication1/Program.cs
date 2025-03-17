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

List<Apple> apples = new List<Apple>
{
    new Apple("������ ������", 120),
    new Apple("������ ������", 150),
    new Apple("������ ������", 130),
    new Apple("������� ������", 100),
    new Apple("������� ������", 110),
    new Apple("������ ������", 140)
};

string appleRest = "apple";

app.MapPost(appleRest, (CreateAppleDTO dto) =>
{
    Apple apple = new Apple(dto.Name ?? "����������� ������", dto.Price ?? 0);
    apples.Add(apple);
    return Results.Created($"/{appleRest}/{apples.IndexOf(apple)}", apple);
});

app.MapPut($"{appleRest}/{{index}}", (UpdateAppleDTO dto, int index) =>
{
    if (index < 0 || index >= apples.Count)
    {
        return Results.NotFound("������ �� �������");
    }

    Apple apple = apples[index];

    if (dto.Name != null) apple.Name = dto.Name;
    if (dto.Price.HasValue) apple.Price = dto.Price.Value;

    return Results.Ok(apple);
});

app.MapDelete($"{appleRest}/{{index}}", (int index) =>
{
    if (index < 0 || index >= apples.Count)
    {
        return Results.NotFound("������ �� �������");
    }

    apples.RemoveAt(index);
    return Results.NoContent();
});

app.MapGet(appleRest, () =>
{
    return Results.Ok(apples);
});

app.Run();

class Apple
{
    public string Name { get; set; }
    public double Price { get; set; }
    public DateTime CreatedDate { get; set; }

    public Apple(string name, double price)
    {
        Name = name;
        Price = price;
        CreatedDate = DateTime.Now;
    }
}

record CreateAppleDTO(string? Name, double? Price);

record UpdateAppleDTO(string? Name, double? Price);
