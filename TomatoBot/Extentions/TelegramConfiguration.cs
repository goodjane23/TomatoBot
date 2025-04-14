using Telegram.Bot;

namespace TomatoBot.Extentions;

public class TelegramBotOptions()
{
    public string Key { get; set; } = "";
}

public static class TelegramConfiguration
{
    public static void AddTelegramBot(this IServiceCollection services, Action<TelegramBotOptions> action)
    {
        var options = new TelegramBotOptions();
        action.Invoke(options);

        var clientOptions = new TelegramBotClientOptions(options.Key);
        var bot = new TelegramBotClient(clientOptions);

        services.AddSingleton<ITelegramBotClient>(bot);
    }    
}