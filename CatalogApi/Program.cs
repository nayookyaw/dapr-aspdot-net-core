var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var items = new Dictionary<string, (string Name, decimal Price)>
{
    ["A001"] = ("Apple", 1.20m),
    ["B002"] = ("Banana", 0.80m),
    ["C003"] = ("Coffee", 4.50m),
};

app.MapGet("/items/{id}", (string id) =>
{
    if (!items.TryGetValue(id, out var item))
        return Results.NotFound(new { message = "Item not found", id });

    return Results.Ok(new { id, item.Name, item.Price });
});

app.Run();