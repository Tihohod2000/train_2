using GrpcService1.Data;
using GrpcService1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Logging.AddConsole();

// регистрация интерфейса
builder.Services.AddScoped<IWagonRepository, WagonRepository>();

var app = builder.Build();

// регистрация сервиса gRPC
app.MapGrpcService<WagonService>();

app.MapGet("/", () => "Use gRPC client to communicate with the server.");

app.Run();