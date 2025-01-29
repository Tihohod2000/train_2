using GrpcService1.Data;
using GrpcService1.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Logging.AddConsole();

// ✅ Правильная регистрация интерфейса с реализацией
builder.Services.AddScoped<IWagonRepository, WagonRepository>();

var app = builder.Build();

// ✅ Правильная регистрация сервиса gRPC
app.MapGrpcService<WagonService>();

app.MapGet("/", () => "Use gRPC client to communicate with the server.");

app.Run();