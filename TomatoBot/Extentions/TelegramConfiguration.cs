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

#if DEBUG
        var clientOptions = "6620828511:AAHkhuFKlX7V1SaPKbOVj-PTZ9GffFyRp2U";
#else
        var clientOptions = new TelegramBotClientOptions(options.Key);
#endif
        var bot = new TelegramBotClient(clientOptions);

        services.AddSingleton<ITelegramBotClient>(bot);
    }    
}