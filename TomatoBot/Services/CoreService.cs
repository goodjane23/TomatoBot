using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TomatoBot.Resources;

namespace TomatoBot.Services;

public class CoreService : IHostedService
{
    private readonly ITelegramBotClient client;
    private readonly GigaChatService gigaService;
    private readonly ILogger<CoreService> logger;
    private Message message;
    private Chat chat;
    private string userName;

    public CoreService(ITelegramBotClient client, GigaChatService gigaService, ILogger<CoreService> logger)
    {
        this.client = client;
        this.gigaService = gigaService;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            client.StartReceiving(UpdateHandler, 
                ErrorHandler,
                cancellationToken:cancellationToken);
            logger.LogInformation("Bot is starting");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
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
                        var procent = TomatoService.GetPercent();
                        res = procent switch
                        {
                            100 => Resources.Strings.Win,
                            50 => Resources.Strings.Half,
                            69 => Resources.Strings.SixtyNine,
                            _ => procent > 50 ? Resources.Strings.MoreThen50 : Resources.Strings.LessThen50,
                        };
                        res = $"@{userName} {Resources.Strings.Starting} {procent}%. {res}";

                        if (procent == 69)
                        {
                            await using var stream = System.IO.File.OpenRead("Resources/Images/ohmy.gif");
                            await client.SendDocument(chat, stream, res);
                            return;
                        }
                        
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
                            try
                            {
                                res = await gigaService.AskAi(prompt);
                                if (string.IsNullOrEmpty(res))
                                {
                                    res = "Зайдите попозже";
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, ex.Message + Environment.NewLine + ex.InnerException?.Message);
                            }
                            
                        }
                        else
                        {
                            res = "Ну напиши че нить";
                        }
                    }
                }
                if (res is not null)
                {
                    _ = await client.SendMessage(chat, res);
                }
            }
        }
        catch (ArgumentException ex)
        {
            _ = await client.SendMessage(chat, ex.Message);
            logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
    private Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        logger.LogError(exception.Message);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Bot is stop");
        return Task.CompletedTask;
    }
}
