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

List<BluetoothShowerHead> showerHeads = new List<BluetoothShowerHead>
{
    new BluetoothShowerHead("Душевая насадка с Bluetooth", 20),
    new BluetoothShowerHead("Насадка с LED подсветкой и Bluetooth", 25),
    new BluetoothShowerHead("Ультра-компактная насадка с Bluetooth", 15)
};

string showerHeadRest = "bluetoothshowerhead";

app.MapPost(showerHeadRest, (CreateBluetoothShowerHeadDTO dto) =>
{
    BluetoothShowerHead showerHead = new BluetoothShowerHead(dto.Name ?? "Насадка для душа без названия", dto.Size ?? 0);
    showerHeads.Add(showerHead);
    return Results.Created($"/{showerHeadRest}/{showerHeads.IndexOf(showerHead)}", showerHead);
});

app.MapPut($"{showerHeadRest}/{{index}}", (UpdateBluetoothShowerHeadDTO dto, int index) =>
{
    if (index < 0 || index >= showerHeads.Count)
    {
        return Results.NotFound("Насадка для душа не найдена");
    }

    BluetoothShowerHead showerHead = showerHeads[index];

    if (dto.Name != null) showerHead.Name = dto.Name;
    if (dto.Size.HasValue) showerHead.Size = dto.Size.Value;

    return Results.Ok(showerHead);
});

app.MapDelete($"{showerHeadRest}/{{index}}", (int index) =>
{
    if (index < 0 || index >= showerHeads.Count)
    {
        return Results.NotFound("Насадка для душа не найдена");
    }

    showerHeads.RemoveAt(index);
    return Results.NoContent();
});

app.MapGet(showerHeadRest, () =>
{
    return Results.Ok(showerHeads);
});

app.Run();

class BluetoothShowerHead
{
    public string Name { get; set; }
    public double Size { get; set; }
    public DateTime CreatedDate { get; set; }

    public BluetoothShowerHead(string name, double size)
    {
        Name = name;
        Size = size;
        CreatedDate = DateTime.Now;
    }
}

record CreateBluetoothShowerHeadDTO(string? Name, double? Size);

record UpdateBluetoothShowerHeadDTO(string? Name, double? Size);
