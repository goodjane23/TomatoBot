
using Microsoft.VisualBasic;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TomatoBot.Resources;

namespace TomatoBot.Services;

public class CoreService : IHostedService
{
    private readonly ITelegramBotClient client;
    private readonly GigaChatService gigaService;
    private Message message;
    private Chat chat;
    private string userName;

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
        if (update.Message is null) return;
        if (update.Message.From is null) return;
        string res = "Каво? Что?";
        try
        {
            message = update.Message;
            chat = message.Chat;
            userName = message.From.Username ?? message.From.FirstName;

            if (update.Type is UpdateType.Message)
            {
                if (message.Text is null) return;
                
                if (message.Text.Contains("/tomat"))
                {
                    if (update.Message.From.FirstName.Equals("WithoutAim"))
                    {
                        res = "Ну ты точно томат! Стопроцентный";
                    }
                    else
                    {
                        res = $"@{userName} {GetString()}";
                    }
                };

                if (message.Text.Contains("/dice"))
                {
                    await client.SendDice(chat);
                    return;
                }

                if (message.Text.Contains("/bread"))
                {
                    Random random = new();
                    BreadDictionary.Bread.TryGetValue(random.Next(0,16), out var val);

                    if (message.From.Id == 289798522)
                    {
                        await client.SendPhoto(
                            chat,
                            InputFile.FromUri(
                                new Uri("https://static.1000.menu/res/640/img/content-v2/a6/37/88873/echpochmak-treugolniki-s-kuricei-i-kartoshkoi-tatarskii_1723464678_0_7pg57nw_max.jpg")),
                            "Эчпочмак горячий с чаем тоже горячим, пиздец вкусно всю жизнь бы ел, брат. Вай мама");
                        return;

                    }

                    res = $"@{userName} cегодня ты {val}";
                };

                if (message.Text.Contains("/askai"))
                {
                    var indexFirstSpace = message.Text.IndexOf(' ');
                    if (indexFirstSpace == -1) res = "Ну напиши че нить";
                    else
                    {
                        var prompt = message.Text.Substring(indexFirstSpace);
                        if (prompt.Length > 0)
                        {
                            res = await gigaService.AskAi(prompt);
                        }
                        else
                        {
                            res = "Ну напиши че нить";
                        }
                    }
                }
                
                _ = await client.SendMessage(chat, res);
            }
        }
        catch (ArgumentException ex)
        {
            _ = await client.SendMessage(chat, ex.Message);          
            Console.WriteLine($"{message} was input");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    private static string GetString()
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
