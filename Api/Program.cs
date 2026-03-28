using System.Reflection;
using Api.Middleware;
using Application.EventHandlers;
using Application.Facades;
using Application.UseCases.Clientes;
using Application.UseCases.Pedidos;
using Application.UseCases.Produtos;
using Application.Validators;
using Domain.Events;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Database;
using Infrastructure.EventPublisher;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AppDbContext as DbContext for use cases
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories (Scoped — Adapters)
builder.Services.AddScoped<IClienteRepository, ClienteRepositoryAdapter>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepositoryAdapter>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepositoryAdapter>();

// Facades
builder.Services.AddScoped<PedidoValidatorService>();
builder.Services.AddScoped<PedidoFactory>();
builder.Services.AddScoped<PedidoFacade>();

// Event Handlers
builder.Services.AddScoped<IEventHandler<PedidoCriado>, PedidoCriadoHandler>();
builder.Services.AddScoped<IEventHandler<PedidoAtualizado>, PedidoAtualizadoHandler>();
builder.Services.AddScoped<IEventHandler<PedidoRemovido>, PedidoRemovidoHandler>();

// Event Publisher
builder.Services.AddScoped<IDomainEventPublisher, InMemoryDomainEventPublisher>();

// Use Cases — Clientes
builder.Services.AddScoped<CreateClienteUseCase>();
builder.Services.AddScoped<GetClienteByIdUseCase>();
builder.Services.AddScoped<GetAllClientesUseCase>();
builder.Services.AddScoped<GetClienteByNomeUseCase>();
builder.Services.AddScoped<UpdateClienteUseCase>();
builder.Services.AddScoped<DeleteClienteUseCase>();
builder.Services.AddScoped<CountClientesUseCase>();

// Use Cases — Produtos
builder.Services.AddScoped<CreateProdutoUseCase>();
builder.Services.AddScoped<GetProdutoByIdUseCase>();
builder.Services.AddScoped<GetAllProdutosUseCase>();
builder.Services.AddScoped<GetProdutoByNomeUseCase>();
builder.Services.AddScoped<UpdateProdutoUseCase>();
builder.Services.AddScoped<DeleteProdutoUseCase>();
builder.Services.AddScoped<CountProdutosUseCase>();

// Use Cases — Pedidos
builder.Services.AddScoped<CreatePedidoUseCase>();
builder.Services.AddScoped<GetPedidoByIdUseCase>();
builder.Services.AddScoped<GetAllPedidosUseCase>();
builder.Services.AddScoped<UpdatePedidoUseCase>();
builder.Services.AddScoped<DeletePedidoUseCase>();
builder.Services.AddScoped<CountPedidosUseCase>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateClienteRequestValidator>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API - Arquitetura Hexagonal",
        Version = "v1",
        Description = "API REST para gestão de Clientes, Produtos e Pedidos"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Apply pending migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
