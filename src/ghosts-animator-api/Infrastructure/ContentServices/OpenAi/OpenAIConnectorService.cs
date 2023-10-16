using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public class OpenAIConnectorService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly OpenAIService _service;
    public bool IsReady { get; set; }

    public OpenAIConnectorService()
    {
        var apiKey = OpenAIHelpers.GetApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            _log.Warn("No OpenAI API key supplied. OpenAIConnectorService not enabled.");
            return;
        }

        HttpClientHandler handler;
        if (!string.IsNullOrEmpty(Program.Configuration.Proxy))
        {
            handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(Program.Configuration.Proxy),
                UseProxy = true
            };
        }
        else
        {
            handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(),
                UseProxy = false
            };
        }

        var client = new HttpClient(handler);

        this._service = new OpenAIService(new OpenAiOptions
        {
            ApiKey = apiKey
        }, client);

        this.IsReady = true;
    }

    //TODO: shouldn't this method save off every request and response somewhere?
    public async Task<string> ExecuteQuery(IList<ChatMessage> messages)
    {
        var completionResult = await this._service.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = OpenAI.GPT3.ObjectModels.Models.Gpt_4,
            //Model = OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo,
            MaxTokens = 500 //optional
            //Temperature = 0.7 //optional
        });

        if (completionResult.Successful)
        {
            var resp = completionResult.Choices.First().Message.Content;
            _log.Trace(resp);
            return resp;
        }

        if (completionResult.Error != null)
        {
            _log.Warn(completionResult.Error.Message);
        }

        return string.Empty;
    }
}