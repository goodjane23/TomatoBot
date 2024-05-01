using System.Diagnostics;
using Telegram.Bot;
using TomatoBot.Services;

var builder = WebApplication.CreateBuilder(args);
Debug.WriteLine($"Билдер");
var key = builder.Configuration["TelegramKey"];
Debug.WriteLine($"Получили ключ");
var client = new TelegramBotClient(key);
Debug.WriteLine($"Инициализация клиента");
var services = builder.Services;
Debug.WriteLine($"Сервисы");

services.AddSingleton<ITelegramBotClient>(client);
services.AddSingleton<TomatoService>();
services.AddHostedService<CoreService>();
Debug.WriteLine($"хостим");
var app = builder.Build();

app.MapGet("/",
    () => Results.Ok("Hello, world"));

app.Run();
