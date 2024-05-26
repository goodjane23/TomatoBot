
using Microsoft.VisualBasic;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TomatoBot.Services;

public class CoreService : IHostedService
{
    private readonly ITelegramBotClient client;
    private readonly GigaChatService gigaService;
    private Message message;
    private Chat chat;

    public CoreService(ITelegramBotClient client, GigaChatService gigaService)
    {
        Debug.WriteLine($"Мы в конструторе ядра");
        this.client = client;
        this.gigaService = gigaService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            client.StartReceiving(UpdateHandler, 
                ErrorHandler,
                cancellationToken:cancellationToken);
            Console.WriteLine($"Bot is starting");
        }
        catch (Exception)
        {

        }
        return Task.CompletedTask;
    }
    
    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {       
        try
        {
            if (update.Message is null) return;

            message = update.Message;
            chat = message.Chat;

            if (update.Type is UpdateType.Message)
            {
                if (message.Text is null) return;
                
                if (message.Text.Contains("/tomat"))
                {
                    if (update.Message.From.FirstName.Equals("WithoutAim"))
                    {
                        var res = "Ну ты точно томат! Стопроцентный";
                        await client.SendTextMessageAsync
                       (chat, res);
                    }
                    else
                    {
                        var res = $"@{message.From.Username} {GetString()}";
                        await client.SendTextMessageAsync
                       (chat, res);
                    }                   
                   
                }
                if (message.Text.Contains("/oracle"))
                {
                    var res = $"@{message.From.Username} {gigaService.SendMessageToAi().Result}";
                    await client.SendTextMessageAsync
                        (chat, res);
                }
            }
        }
        catch (ArgumentException ex)
        {
            await client.SendTextMessageAsync(
                        chat.Id, ex.Message);
            Console.WriteLine($"{message} was input");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    private string GetString()
    {
        var procent = TomatoService.GetPercent();

        if (procent == 100)
            return $"{Resources.Strings.Starting} {procent}%. {Resources.Strings.Win}";

        if (procent == 50)
            return $"{Resources.Strings.Starting} {procent}%. {Resources.Strings.Half}";

        if (procent > 50)
            return $"{Resources.Strings.Starting} {procent}%. {Resources.Strings.MoreThen50}";

        return $"{Resources.Strings.Starting} {procent}%. {Resources.Strings.LessThen50}";
    }

    private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
