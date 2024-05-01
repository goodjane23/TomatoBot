using System.Diagnostics;
using Telegram.Bot;
using TomatoBot.Services;

var builder = WebApplication.CreateBuilder(args);
Debug.WriteLine($"������");
var key = builder.Configuration["TelegramKey"];
Debug.WriteLine($"�������� ����");
var client = new TelegramBotClient(key);
Debug.WriteLine($"������������� �������");
var services = builder.Services;
Debug.WriteLine($"�������");

services.AddSingleton<ITelegramBotClient>(client);
services.AddSingleton<TomatoService>();
services.AddHostedService<CoreService>();
Debug.WriteLine($"������");
var app = builder.Build();

app.MapGet("/",
    () => Results.Ok("Hello, world"));

app.Run();
