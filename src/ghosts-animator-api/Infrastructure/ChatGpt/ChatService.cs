using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Ghosts.Animator.Api.Infrastructure.ChatGpt;

public class ChatService
{
    /// <summary>
    /// You'll need a .env file at config/.env with this line:
    /// chatgtp_api_key:your_key_here
    /// </summary>
    /// <returns></returns>
    public static string GetApiKey()
    {
        string retVal = null;
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config", ".env");
        
        if(File.Exists(path))
        {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!line.StartsWith("chatgtp_api_key")) continue;
                retVal = line.Replace("chatgtp_api_key:", "");
                break;
            }
        }

        return retVal;
    }
    
    public static string Get(NPC agent)
    {
        var key = GetApiKey();
        if (string.IsNullOrEmpty(key))
            return null;
        
        var flattenedAgent = JsonConvert.SerializeObject(agent);
        flattenedAgent = flattenedAgent.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" \"", "").Replace("\"", "");
            
        var client = new RestClient("https://api.openai.com/");
        var request = new RestRequest("v1/completions", Method.Post)
        {
            RequestFormat = DataFormat.Json
        };
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {key}");
        var body = $@"{{""model"": ""text-davinci-003"", ""prompt"": ""Given this json file about a person ```{flattenedAgent[..3050]}``` Write something they might tweet"", ""temperature"": 0, ""max_tokens"": 1024}}";
        request.AddBody(body);

        try
        {
            var response = client.Execute(request);
            if(response.StatusCode != HttpStatusCode.OK)
                throw(new Exception($"chat api responded with {response.StatusCode}"));
            
            Console.WriteLine(response.Content);
            var o = JsonConvert.DeserializeObject<ChatGptResponse>(response.Content);
            var text = o.choices[0].text;
            text = Regex.Replace(text, @"\t|\n|\r", "").Replace("\"", "");
            return text;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not post timeline command to chat api: {e}");
        }

        return null;
    }
    
    // Root myDeserializedClass = JsonConvert.DeserializeObject<ChatGptResponse>(myJsonResponse);
    public class Choice
    {
        public string text { get; set; }
        public int index { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class ChatGptResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}