// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Net;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using NLog;
using RestSharp;

namespace Ghosts.Animator.Api.Infrastructure.Services;

public class GhostsApiService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();

    public static async Task PostTimeline(ApplicationConfiguration configuration, string payload)
    {
        _log.Trace(payload);

        var client = new RestClient(configuration.GhostsApiUrl);
        var request = new RestRequest("api/machineupdates", Method.Post)
        {
            RequestFormat = DataFormat.Json
        };
        request.AddBody(payload);

        try
        {
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw (new Exception($"GHOSTS API responded with {response.StatusCode}"));
        }
        catch (Exception e)
        {
            _log.Error($"Could not post timeline to GHOSTS API {configuration.GhostsApiUrl}: {e}");
        }
    }
}