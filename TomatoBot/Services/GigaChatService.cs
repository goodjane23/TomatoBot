using GigaChatAdapter;
using TomatoBot.Options;
using GigaChatAdapter.Auth;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace TomatoBot.Services;

public class GigaChatService
{
    private readonly string response;
    private readonly Authorization auth;
    private AuthorizationResponse authResult;

    public GigaChatService(IOptions<KeysOptions> options)
    {
        auth = new Authorization(options.Value.GigaChatKey, RateScope.GIGACHAT_API_PERS);
    }

    public async Task<string?> AskAi(string prompt)
    {
        authResult = await auth.SendRequest();
        if (authResult.AuthorizationSuccess)
        {
            Completion completion = new(); 
            await auth.UpdateToken(reserveTime: new TimeSpan(0, 1, 0));

            var result = await completion.SendRequest(auth.LastResponse.GigaChatAuthorizationResponse?.AccessToken, prompt);
            
            if (result.RequestSuccessed)
            {
                return result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content;
            }
        }
        else
        {
            Debug.WriteLine($"Authorization has fallen: {authResult.ErrorTextIfFailed}");            
        }
        return null;
    }
}
