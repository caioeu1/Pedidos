using Microsoft.EntityFrameworkCore;
using Orders.API.Middleware;
using Orders.Application.Interfaces;
using Orders.Application.UseCases.CreateOrder;
using Orders.Application.UseCases.GetOrder;
using Orders.Application.UseCases.UpdateOrderStatus;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Plataforma de Pedidos API", Version = "v1" });
});

// Banco de dados (SQLite para rodar sem configuração)
builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("Default");
    if (conn != null && conn.Contains("Server="))
        options.UseSqlServer(conn);
    else
        options.UseSqlite(conn ?? "Data Source=orders.db");
});

// Repositórios e Unit of Work
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Use Cases (Handlers)
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<GetOrderHandler>();
builder.Services.AddScoped<UpdateOrderStatusHandler>();

var app = builder.Build();

// Criar banco automaticamente na primeira execução
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureCreated();
}

// Middleware de tratamento de erros
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger sempre habilitado para facilitar testes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000
});

app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }