using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient(); // to call Dapr sidecar

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Dapr sidecar HTTP endpoint for this app will be http://localhost:3502
// (we will run it with --dapr-http-port 3502)
const string daprSidecar = "http://localhost:3501";

// 1) Call Catalog API via Dapr service invocation
app.MapGet("/proxy/catalog/{id}", async (string id, IHttpClientFactory httpFactory) =>
{
    var http = httpFactory.CreateClient();

    var url = $"http://localhost:3501/v1.0/invoke/catalog-api/method/items/{id}";
    Console.WriteLine($"Calling: {url}");

    var response = await http.GetAsync(url);
    var body = await response.Content.ReadAsStringAsync();

    Console.WriteLine($"Status: {(int)response.StatusCode}");

    // Return body regardless of status
    return Results.Content(body, response.Content.Headers.ContentType?.ToString() ?? "application/json",
        statusCode: (int)response.StatusCode);
});

// 2) Save user cart to Dapr state store (Redis)
app.MapPost("/cart/{userId}", async (string userId, Cart cart, IHttpClientFactory httpFactory) =>
{
    var http = httpFactory.CreateClient();

    // Dapr state save endpoint:
    // POST /v1.0/state/{storeName}
    // body: [{ "key": "...", "value": ... }]
    var body = new[]
    {
        new { key = $"cart-{userId}", value = cart }
    };

    var resp = await http.PostAsJsonAsync($"{daprSidecar}/v1.0/state/statestore", body);
    resp.EnsureSuccessStatusCode();

    return Results.Ok(new { message = "Saved", userId });
});

// 3) Read cart from Dapr state store
app.MapGet("/cart/{userId}", async (string userId, IHttpClientFactory httpFactory) =>
{
    var http = httpFactory.CreateClient();
    var url = $"{daprSidecar}/v1.0/state/statestore/cart-{userId}";
    var cart = await http.GetFromJsonAsync<Cart>(url) ?? new Cart();
    return Results.Ok(cart);
});

app.MapControllers();
app.Run();

public class Cart
{
    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public string ItemId { get; set; } = "";
    public int Quantity { get; set; }
}