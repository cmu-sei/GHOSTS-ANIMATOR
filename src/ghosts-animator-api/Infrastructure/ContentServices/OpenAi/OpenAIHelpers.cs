using System;
using System.IO;
using System.Linq;
using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public static class OpenAiHelpers
{
    /// <summary>
    /// You'll need to supply your openAi api key via an environment variable
    /// </summary>
    public static string GetApiKey()
    {
        return Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
    }
}