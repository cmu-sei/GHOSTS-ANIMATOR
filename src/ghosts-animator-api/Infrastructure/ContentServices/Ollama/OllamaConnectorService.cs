using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;

public class OllamaConnectorService
{
    private static readonly string OLLAMA_HOST = Environment.GetEnvironmentVariable("OLLAMA_HOST") ?? "http://localhost:11434";
    private static readonly string OLLAMA_MODEL = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama2:13b";
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();

    public async Task<string> ExecuteQuery(string prompt)
    {
        return await ExecuteQuery(OLLAMA_MODEL, prompt);
    }
    
    public async Task<string> ExecuteQuery(string modelName, string prompt, string system = null, string template = null, string context = null, string options = null, Action<string> callback = null)
    {
        try
        {
            var url = $"{OLLAMA_HOST}/api/generate";
            var payload = new Dictionary<string, string>
            {
                { "model", modelName },
                { "prompt", prompt },
                { "system", system },
                { "template", template },
                { "context", context },
                { "options", options }
            };

            payload = payload.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            using var client = new HttpClient();
            client.Timeout = client.Timeout * 10;
            using var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var fullResponse = new StringBuilder();

            using var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync());
            while (await streamReader.ReadLineAsync() is { } line)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    dynamic json = JsonConvert.DeserializeObject(line);
                    if (json != null && json["done"] != true)
                    {
                        fullResponse.Append(json["response"]);
                    }
                }
            }

            return fullResponse.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _log.Error($"Ollama threw an exception: {ex.Message}: {ex.StackTrace}");
            return null;
        }
    }
}