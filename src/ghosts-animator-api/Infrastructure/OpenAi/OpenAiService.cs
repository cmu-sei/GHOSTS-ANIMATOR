using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Ghosts.Animator.Api.Infrastructure.OpenAi;

public class OpenAiService
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

        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!line.StartsWith("open_ai_api_key")) continue;
                retVal = line.Replace("open_ai_api_key:", "");
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

        agent.Campaign = null;
        agent.Enclave = null;
        agent.Team = null;
        agent.Rank = null;
        agent.CAC = null;
        agent.PGP = null;
        agent.Unit = null;

        var flattenedAgent = JsonConvert.SerializeObject(agent);
        flattenedAgent = flattenedAgent.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" \"", "").Replace("\"", "");

        return Chat.Process(key, flattenedAgent);
    }

    public class Chat
    {
        public static string Process(string key, string flattenedAgent)
        {
            var resource = "v1/chat/completions";
            var gptQuery = new Request.Gpt();
            gptQuery.model = "gpt-4";//"gpt-3.5-turbo";
            gptQuery.messages.Add(new Request.Message
            {
                role = "user",
                content =
                    $"Given this json file about a person ```{flattenedAgent[..3050]}``` Write something they might tweet. not too many (if any) hashtags. and don't always start them with the word just"
            });
            var body = JsonConvert.SerializeObject(gptQuery);
            //body = $@"{{""model"": ""gpt-3.5-turbo"", ""prompt"": ""Given this json file about a person ```{flattenedAgent[..3050]}``` Write something they might tweet"", ""temperature"": 0, ""max_tokens"": 1024}}";

            var client = new RestClient("https://api.openai.com/");
            var request = new RestRequest(resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {key}");
            request.AddBody(body);

            try
            {
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw (new Exception($"chat api responded with {response.StatusCode}"));

                Console.WriteLine(response.Content);
                var o = JsonConvert.DeserializeObject<Response.GptResponse>(response.Content);
                var text = o.choices[0].message.content;
                text = Regex.Replace(text, @"\t|\n|\r", "").Replace("\"", "");
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not post timeline command to chat api: {e}");
            }

            return null;
        }


        public class Request
        {
            public class Message
            {
                public string role { get; set; }
                public string content { get; set; }
            }

            public class Gpt
            {
                public string model { get; set; }
                public List<Message> messages { get; set; }

                public Gpt()
                {
                    this.messages = new List<Message>();
                }
            }
        }

        public class Response
        {
            public class Choice
            {
                public int index { get; set; }
                public Message message { get; set; }
                public string finish_reason { get; set; }
            }

            public class Message
            {
                public string role { get; set; }
                public string content { get; set; }
            }

            public class GptResponse
            {
                public string id { get; set; }
                public string @object { get; set; }
                public int created { get; set; }
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
    }


    public class Completions
    {
        public static string Process(string key, string flattenedAgent)
        {
            //davinci
            var resource = "v1/completions";
            var body =
                $@"{{""model"": ""text-davinci-003"", ""prompt"": ""Given this json file about a person ```{flattenedAgent[..3050]}``` Write something they might tweet"", ""temperature"": 0, ""max_tokens"": 1024}}";

            var client = new RestClient("https://api.openai.com/");
            var request = new RestRequest(resource, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {key}");
            request.AddBody(body);

            try
            {
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw (new Exception($"chat api responded with {response.StatusCode}"));

                Console.WriteLine(response.Content);
                var o = JsonConvert.DeserializeObject<CompletionsResponse>(response.Content);
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

        public class Choice
        {
            public string text { get; set; }
            public int index { get; set; }
            public object logprobs { get; set; }
            public string finish_reason { get; set; }
        }

        public class CompletionsResponse
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
}