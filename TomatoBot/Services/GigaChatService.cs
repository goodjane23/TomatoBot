using GigaChatAdapter;
using TomatoBot.Options;
using GigaChatAdapter.Auth;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace TomatoBot.Services;

public class GigaChatService
{
    private readonly Authorization auth;
    private readonly ILogger<GigaChatService> logger;
    private AuthorizationResponse authResult;

    public GigaChatService(IOptions<KeysOptions> options, ILogger<GigaChatService> logger)
    {
        auth = new Authorization(options.Value.GigaChatKey, RateScope.GIGACHAT_API_PERS);
        this.logger = logger;
    }

    public async Task<string?> AskAi(string prompt)
    {
        authResult = await auth.SendRequest();
        if (authResult.AuthorizationSuccess)
        {
            Completion completion = new(); 
            var resultUpdateToken =  await auth.UpdateToken(reserveTime: new TimeSpan(0, 1, 0));

            var result = await completion.SendRequest(auth.LastResponse.GigaChatAuthorizationResponse?.AccessToken, prompt);
            
            if (result.RequestSuccessed)
            {
                return result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content;
            }
        }
        else
        {
            logger.LogWarning($"Authorization has fallen: {authResult.ErrorTextIfFailed}");            
        }
        return null;
    }
}
