using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyCustomList.Data;
using MyCustomList.Models;
using MyCustomList.Validators;

var builder = WebApplication.CreateBuilder(args);

// Ajouter EF Core avec SQLite
builder.Services.AddDbContext<MyCustomListContext>(options =>
    options.UseNpgsql("Host=dpg-d0utn0euk2gs73aug580-a;Port=5432;Database=dbmycustomlist_0j50;Username=dbmycustomlist_user;Password=A1M9qOtjDmXwaMaUQJ11lYv5DlxJX7L7;SSL Mode=Require;Trust Server Certificate=true;"));

builder.Services.AddEndpointsApiExplorer(); // 👈 nécessaire pour Minimal API
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
builder.Services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyCustomListContext>();
    db.Database.EnsureCreated();
}
// CRUD Products

// GET all
app.MapGet("/products", async (MyCustomListContext db) =>
    await db.Products.ToListAsync());

// GET by id
app.MapGet("/products/{id}", async (int id, MyCustomListContext db) =>
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound());

// GET by query string
app.MapGet("/products/search", async (string? name, MyCustomListContext db) =>
{
    var products = await db.Products
        .Where(p => (string.IsNullOrEmpty(name) || p.Name.Contains(name)) &&
                !db.MyProducts.Any(mp => mp.Id == p.Id))
        .ToListAsync();
    return Results.Ok(products);
});

// POST create
app.MapPost("/products", async (ProductDto dto, MyCustomListContext db, IValidator<ProductDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var product = new Product
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        ImageUrl = dto.ImageUrl
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// PUT update
app.MapPut("/products/{id}", async (int id, ProductDto dto, MyCustomListContext db, IValidator<ProductDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = dto.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE
app.MapDelete("/products/{id}", async (int id, MyCustomListContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// CRUD MyProducts
// CRUD

// GET all
app.MapGet("/myproducts", async (MyCustomListContext db) =>
    await db.MyProducts.ToListAsync());

// GET by id
app.MapGet("/myproducts/{id}", async (int id, MyCustomListContext db) =>
    await db.MyProducts.FindAsync(id) is MyProduct product
        ? Results.Ok(product)
        : Results.NotFound());

// POST create
app.MapPost("/myproducts", async (ProductDto dto, MyCustomListContext db, IValidator<ProductDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var product = new MyProduct
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        ImageUrl = dto.ImageUrl
    };

    db.MyProducts.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/myproducts/{product.Id}", product);
});

// PUT update
app.MapPut("/myproducts/{id}", async (int id, ProductDto dto, MyCustomListContext db, IValidator<ProductDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var product = await db.MyProducts.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = dto.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE
app.MapDelete("/myproducts/{id}", async (int id, MyCustomListContext db) =>
{
    var product = await db.MyProducts.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.MyProducts.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
