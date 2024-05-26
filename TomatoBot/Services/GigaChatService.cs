﻿using GigaChatAdapter;
using GigaChatAdapter.Auth;
using Microsoft.Extensions.Options;
using TomatoBot.Options;

namespace TomatoBot.Services;

public class GigaChatService
{
    private readonly string key = "";
    private readonly KeysOptions options;
    private const string FUNNYPREDICTIONPROMPT = "Придумай веселое предсказание для гуся";
    private const string TOXICPREDICTIONPROMPT = "Придумай токсичное саркастичное предсказание для гуся";
    private string response;

    private Authorization auth;
    private AuthorizationResponse authResult;

    public GigaChatService(IOptions<KeysOptions> options)
    {
        this.options = options.Value;
        Auth();
    }
    private async Task Auth()
    {
        auth = new Authorization(options.GigaChatKey, RateScope.GIGACHAT_API_PERS);
        authResult = await auth.SendRequest();
    }

    public async Task<string> SendMessageToAi()
    {
        if (authResult.AuthorizationSuccess)
        {
            Completion completion = new(); //Обновление токена, если он просрочился
            await auth.UpdateToken();

            var r = new Random().Next(0, 2);

            string prompt = r == 0 ? FUNNYPREDICTIONPROMPT : TOXICPREDICTIONPROMPT;
            //отправка промпта
            var result = await completion.SendRequest(auth.LastResponse.GigaChatAuthorizationResponse?.AccessToken, prompt);
            if (result.RequestSuccessed)
            {
                response = result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content;
            }
        }
        return response;
    }

}
