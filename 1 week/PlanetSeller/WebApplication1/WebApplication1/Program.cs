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

List<Planet> planets = new List<Planet>
{
    new Planet("Земля", 900000000),
    new Planet("Марс", 12300000001315),
    new Planet("Юпитер", 1308946809324506),
    new Planet("Меркурий", 1230495633249856),
    new Planet("Солнце", 8943589425892498235),
    new Planet("Плутон", 103847251938475)

};

string planetRest = "planet";

app.MapPost(planetRest, (CreatePlanetDTO dto) =>
{
    Planet planet = new Planet(dto.Name ?? "Неизвестная планета", dto.Price ?? 0);
    planets.Add(planet);
    return Results.Created($"/{planetRest}/{planets.IndexOf(planet)}", planet);
});

app.MapPut($"{planetRest}/{{index}}", (UpdatePlanetDTO dto, int index) =>
{
    if (index < 0 || index >= planets.Count)
    {
        return Results.NotFound("Планета не найдена");
    }

    Planet planet = planets[index];

    if (dto.Name != null) planet.Name = dto.Name;
    if (dto.Price.HasValue) planet.Price = dto.Price.Value;

    return Results.Ok(planet);
});

app.MapDelete($"{planetRest}/{{index}}", (int index) =>
{
    if (index < 0 || index >= planets.Count)
    {
        return Results.NotFound("Планета не найдена");
    }

    planets.RemoveAt(index);
    return Results.NoContent();
});

app.MapGet(planetRest, () =>
{
    return Results.Ok(planets);
});

app.Run();

class Planet
{
    public string Name { get; set; }
    public double Price { get; set; }
    public DateTime CreatedDate { get; set; }

    public Planet(string name, double price)
    {
        Name = name;
        Price = price;
        CreatedDate = DateTime.Now;
    }
}

record CreatePlanetDTO(string? Name, double? Price);

record UpdatePlanetDTO(string? Name, double? Price);
