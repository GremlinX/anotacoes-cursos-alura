using backend_wishlist.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext para usar PostgreSQL com a string de conexão
builder.Services.AddDbContext<WishlistContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WishlistDatabase")));

// Adiciona os serviços necessários para usar controladores
builder.Services.AddControllers();

// Permite usar a Biblioteca Auto Mapper
builder.Services.AddAutoMapper(typeof(Program));

// 1. Defina a política CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // URL do front-end
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 2. Use a política CORS
app.UseCors("AllowVueApp");

// Configura o uso de controladores
app.MapControllers();

// Inicia a aplicação
app.Run();
