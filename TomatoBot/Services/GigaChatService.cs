using GigaChatAdapter;
using GigaChatAdapter.Auth;
using Microsoft.Extensions.Options;
using TomatoBot.Options;

namespace TomatoBot.Services;

public class GigaChatService
{
    private readonly KeysOptions options;
    private const string FUNNYPREDICTIONPROMPT = "Придумай веселое предсказание для гуся";
    private string response;

    private Authorization auth;
    private AuthorizationResponse authResult;

    public GigaChatService(IOptions<KeysOptions> options)
    {
        auth = new Authorization(options.Value.GigaChatKey, RateScope.GIGACHAT_API_PERS);
    }

    public async Task<string> AskAi(string prompt)
    {
        authResult = await auth.SendRequest();
        if (authResult.AuthorizationSuccess)
        {
            Completion completion = new(); //Обновление токена, если он просрочился
            await auth.UpdateToken();
            
            var result = await completion.SendRequest(auth.LastResponse.GigaChatAuthorizationResponse?.AccessToken, prompt);
            
            if (result.RequestSuccessed)
            {
                response = result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content;
            }
        }
        return response;
    }

    public async Task<string> SendMessageToAi()
    {
        if (authResult.AuthorizationSuccess)
        {
            Completion completion = new(); //Обновление токена, если он просрочился
            await auth.UpdateToken();

            var r = new Random().Next(0, 2);

            string prompt = FUNNYPREDICTIONPROMPT;
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
