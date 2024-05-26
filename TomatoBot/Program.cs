using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Telegram.Bot;
using TomatoBot.Extentions;
using TomatoBot.Options;
using TomatoBot.Services;

var builder = WebApplication.CreateBuilder(args);
var keysSection = builder.Configuration.GetSection("Keys");
var services = builder.Services;
services.Configure<KeysOptions>(keysSection);
services.AddTelegramBot(options => options.Key = builder.Configuration["Keys:TelegramKey"]);
services.AddSingleton<TomatoService>();
services.AddSingleton<GigaChatService>();
services.AddHostedService<CoreService>();

var app = builder.Build();

app.MapGet("/",
    () => Results.Ok("Hello, world"));

app.Run();
