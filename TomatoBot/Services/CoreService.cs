using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TomatoBot.Models;
using TomatoBot.Resources;

namespace TomatoBot.Services;

public class CoreService : IHostedService
{
    private readonly ITelegramBotClient client;
    private readonly GigaChatService gigaService;
    private readonly ILogger<CoreService> logger;   
    
    private string userName;
    private int randCheckNum;
   // private ConcurrentDictionary<long, int> usersButtons = new();
    private ConcurrentDictionary<long, CaptchaData> userCaptcha  = new();
    private Message capchaMessage;

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
        switch (update.Type)
        {
            case UpdateType.Message:
                await MessageHandler(update);
                break;
            case UpdateType.CallbackQuery:
                await CallbackQueryHandler(update);
                break;
            default:
                break;
        }
    }

    private async Task CallbackQueryHandler(Update update)
    {
        int res = -1;
        try
        {
            var chat = update.CallbackQuery.Message.Chat;
            var userId = update.CallbackQuery.From.Id;
            var cbd = update.CallbackQuery.Data;

            var isUserExist = userCaptcha.TryGetValue(userId, out var data);
            if (!isUserExist) return;

            _ = int.TryParse(cbd, out res);
            if (res != data.ExpectedValue)
            {
                await client.BanChatMember(chat, update.CallbackQuery.From.Id);
            }

            await client.DeleteMessage(chat.Id, data.MessageId);
            userCaptcha.TryRemove(userId, out _);
        }
        catch (Exception ex)
        {
            logger.LogError("Ошбика в методе CallbackQueryHandler: " + ex.Message + "CallbackQuery.Data:" + res);
        }
    }

    private async Task MessageHandler(Update update)
    {
        string res = string.Empty;
        try
        {
            var message = update.Message;
            var chat = message.Chat;
            userName = message.From.Username ?? message.From.FirstName;

            if (update.Type is UpdateType.Message)
            {
                if (userCaptcha.TryGetValue(message.From.Id, out _))
                {
                    await client.DeleteMessage(chat.Id, message.Id);
                }
                if (message.Type == MessageType.NewChatMembers)
                {
                    await CheckUser(update);
                }

                if (message.Text is null) return;

                if (message.Text.Contains("/tomat"))
                {
                    if (update.Message.From.FirstName.Equals("WithoutAim"))
                    {
                        res = "Ты точно томат! Стопроцентный";
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
                }               

                if (message.Text.Contains("/dice"))
                {
                    await client.SendDice(chat);
                    return;
                }
                if (message.Text.Contains("/coin"))
                {
                    if (Random.Shared.Next(1, 3) == 1)
                    {
                        await client.SendSticker(chat, InputFile.FromString("CAACAgIAAxkBAAK7ZWl5EEiVy1A-D1n__nfl-OBzzsHMAAIZbwACXxxpSl9IHKfngYQPOAQ"));
                    }
                    else
                    {
                        await client.SendSticker(chat, InputFile.FromString("CAACAgIAAxkBAAK7a2l5EFB-JYsLhRjgfd8aJtM_B06IAAJEcgACxuZxShUDbdPs6G2lOAQ"));
                    }
                    return;
                }
                if (message.Text.Contains("/bread"))
                {
                    BreadDictionary.Bread.TryGetValue(Random.Shared.Next(0, 16), out var val);

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

                if (message.Text.Contains("/flex"))
                {
                    var today = DateTime.Today;
                    DateTime targetDate = new(2026, 11, 21);

                    var difference = targetDate - today;
                    var daysDifference = (int)difference.TotalDays;

                    res = $"До флекса осталось {daysDifference} д";
                }
                if (!string.IsNullOrEmpty(res))
                {
                    _ = await client.SendMessage(chat, res);
                }
            }
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }

    private async Task CheckUser(Update update)
    {
        var chat = update.Message.Chat;
        randCheckNum = Random.Shared.Next(0,8);
      
        var buttons = new List<InlineKeyboardButton>();
        for (int i = 0; i <8; i++)
        {
            buttons.Add(new() { Text = $"{i}", CallbackData = $"{i}" });
        }
        var buttonsLine = new List<List<InlineKeyboardButton>>
        {
            buttons
        };
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(buttonsLine);

        var messageId = await client.SendMessage(
            chat, 
            $"Привет, @{userName}, нажми на цифру {randCheckNum}", replyMarkup: inlineKeyboardMarkup
            );

        userCaptcha.TryAdd(update.Message.From.Id, new() { ExpectedValue = randCheckNum, MessageId = messageId.Id });
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
