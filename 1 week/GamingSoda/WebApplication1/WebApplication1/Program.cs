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

List<Soda> sodas = new List<Soda>
{
    new Soda("����", 50),
    new Soda("���� ��� ����", 30),
    new Soda("����-�������������", 40),
    new Soda("��������� ����", 60),
    new Soda("���� � RGB �����������", 45),
    new Soda("���� ��� �����������", 55)
};

string sodaRest = "soda";

app.MapPost(sodaRest, (CreateSodaDTO dto) =>
{
    Soda soda = new Soda(dto.Name ?? "����������� ����", dto.Price ?? 0);
    sodas.Add(soda);
    return Results.Created($"/{sodaRest}/{sodas.IndexOf(soda)}", soda);
});

app.MapPut($"{sodaRest}/{{index}}", (UpdateSodaDTO dto, int index) =>
{
    if (index < 0 || index >= sodas.Count)
    {
        return Results.NotFound("���� �� �������");
    }

    Soda soda = sodas[index];

    if (dto.Name != null) soda.Name = dto.Name;
    if (dto.Price.HasValue) soda.Price = dto.Price.Value;

    return Results.Ok(soda);
});

app.MapDelete($"{sodaRest}/{{index}}", (int index) =>
{
    if (index < 0 || index >= sodas.Count)
    {
        return Results.NotFound("���� �� �������");
    }

    sodas.RemoveAt(index);
    return Results.NoContent();
});

app.MapGet(sodaRest, () =>
{
    return Results.Ok(sodas);
});

app.Run();

class Soda
{
    public string Name { get; set; }
    public double Price { get; set; }
    public DateTime CreatedDate { get; set; }

    public Soda(string name, double price)
    {
        Name = name;
        Price = price;
        CreatedDate = DateTime.Now;
    }
}

record CreateSodaDTO(string? Name, double? Price);

record UpdateSodaDTO(string? Name, double? Price);
