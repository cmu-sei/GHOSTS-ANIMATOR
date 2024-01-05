﻿// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions.Chat.Mattermost;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions.Chat;

public class ChatClient
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ChatJobConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly HttpClient _client;
    private string _token;
    public string UserId { get; private set; }

    public ChatClient(ChatJobConfiguration config)
    {
        _configuration = config;
        this._baseUrl = _configuration.Chat.BaseUrl;
        this._client = new HttpClient();
    }

    public async Task<User> AdminLogin()
    {
        return await this.Login(this._configuration.Chat.AdminUsername,
            this._configuration.Chat.AdminPassword);
    }

    public async Task<User> Login(string username, string password)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}users/login");
            var content = new StringContent($"{{\"login_id\":\"{username}\",\"password\":\"{password}\"}}", null,
                "application/json");
            request.Content = content;
            var response = await this._client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Reading the 'Token' value from response headers
            if (response.Headers.TryGetValues("Token", out var values))
            {
                this._token = values.First(); // Assuming there's at least one value
                this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._token);
            }
            else
            {
                throw new Exception("Token not found in the response headers.");
            }

            var contentString = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(contentString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (user == null)
                throw new Exception("A user was not returned properly");
            this.UserId = user.Id;
            return user;
        }
        catch (Exception e)
        {
            _log.Error(e);
            throw;
        }
    }

    public async Task<IEnumerable<Team>> GetMyTeams()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/me/teams");
        var content = await ExecuteRequest(request);

        try
        {
            var response = JsonSerializer.Deserialize<IEnumerable<Team>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return response;
        }
        catch (Exception e)
        {
            _log.Error($"No teams found {e}");
            return new List<Team>();
        }
    }

    public async Task<IEnumerable<Channel>> GetMyChannels()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/me/channels");
        var content = await ExecuteRequest(request);

        try
        {
            var response = JsonSerializer.Deserialize<IEnumerable<Channel>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return response;
        }
        catch (Exception e)
        {
            _log.Error($"No channels found {e}");
            return new List<Channel>();
        }
    }

    public async Task CreateUser(UserCreate create)
    {
        var jsonPayload = JsonSerializer.Serialize(create.ToObject());
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}users")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        await ExecuteRequest(request);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users?");
        var content = await ExecuteRequest(request);

        var response = JsonSerializer.Deserialize<IEnumerable<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return response;
    }

    public async Task<User> GetUserByUsername(string username)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/username/{username}");
        var content = await ExecuteRequest(request);

        var response = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return response;
    }
    
    public async Task<User> GetUserById(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/{id}");
        var content = await ExecuteRequest(request);

        var response = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return response;
    }

    public async Task JoinTeam(string userId, string teamId)
    {
        var payload = new
        {
            team_id = teamId,
            user_id = userId
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}teams/{teamId}/members")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        await ExecuteRequest(request);
    }

    public async Task JoinChannel(string userId, string channelId)
    {
        var payload = new
        {
            user_id = userId
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}channels/{channelId}/members")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        await ExecuteRequest(request);
    }

    public async Task<IEnumerable<Team>> GetTeams()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}teams");
        var content = await ExecuteRequest(request);

        var response = JsonSerializer.Deserialize<IEnumerable<Team>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return response;
    }

    public async Task<IEnumerable<Channel>> GetChannelsByTeam(string teamId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}teams/{teamId}/channels");
        var content = await ExecuteRequest(request);

        try
        {
            var response = JsonSerializer.Deserialize<IEnumerable<Channel>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return response;
        }
        catch (Exception e)
        {
            _log.Error($"No channels found {e}");
            return new List<Channel>();
        }
    }

    // public async Task GetUnreadPosts()
    // {
    //     var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/me/teams/unread");
    //     var content = await ExecuteRequest(request);
    //     //TODO
    // }
    //
    // public async Task GetUnreadPostsByTeam(string teamId)
    // {
    //     var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}users/me/teams/{teamId}/unread");
    //     var content = await ExecuteRequest(request);
    //     //TODO
    // }

    public async Task<PostResponse> GetPostsByChannel(string channelId, string afterPostId = "")
    {
        var request =
            new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}channels/{channelId}/posts?after={afterPostId}");
        var content = await ExecuteRequest(request);

        var response = JsonSerializer.Deserialize<PostResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return response;
    }


    private async Task<Post> CreatePost(string channelId, string m)
    {
        // JSON payload
        var payload = new
        {
            channel_id = channelId,
            message = m
        };

        // Serialize the payload to JSON
        var jsonPayload = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}posts?set_online=true")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        var content = await ExecuteRequest(request);
        if (!string.IsNullOrEmpty(content))
        {
            try
            {
                var response =
                    JsonSerializer.Deserialize<Post>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return response;
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        return new Post();
    }

    private async Task<string> ExecuteRequest(HttpRequestMessage request)
    {
        try
        {
            var response = await this._client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest)
            {
                _log.Info(await response.Content.ReadAsStringAsync());
                return string.Empty;
            }

            response.EnsureSuccessStatusCode();
            var contentString = await response.Content.ReadAsStringAsync();

            return contentString;
        }
        catch (Exception e)
        {
            _log.Error(request.RequestUri != null
                ? $"Error with {request.Method} request {request.RequestUri.ToString()} : {e}"
                : $"Error with {request.Method} request to null uri : {e}");
            return string.Empty;
        }
    }

    public async Task Step(OllamaConnectorService llm, Random random, IEnumerable<NPC> agents)
    {
        await this.AdminLogin();
        
        var agentsWithAccounts = await this.GetUsers();
        var agentsWithAccountsHash = new HashSet<string>(agentsWithAccounts.Select(a => a.Email));
        var agentList = agents.ToList();
        foreach (var agent in agentList)
        {
            var username = agent.Email.CreateUsernameFromEmail();
            if (!agentsWithAccountsHash.Contains(agent.Email))
            {
                await this.CreateUser(new UserCreate
                {
                    Email = agent.Email, FirstName = agent.Name.First, LastName = agent.Name.Last,
                    Nickname = agent.Name.ToString() ?? string.Empty, Password = _configuration.Chat.DefaultUserPassword, Username =  username
                });
                
                await this.StepEx(llm, random, username, _configuration.Chat.DefaultUserPassword);
            }
        }
    }

    private async Task StepEx(OllamaConnectorService llm, Random random, string username, string password)
    {
        _log.Trace($"Managing {username}...");

        try
        {
            await this.Login(username, password);
            _log.Trace($"{username} is now logged in");
        }
        catch (Exception e)
        {
            _log.Warn($"Could not login {username}, {password} with: {e}");
            return;
        }

        var me = await this.GetUserByUsername(username);
        var channelHistory = new List<ChannelHistory>();
        try
        {
            while (true)
            {
                var myTeams = await this.GetMyTeams();
                var myChannels = await this.GetMyChannels();
                var myChannelsList = myChannels as Channel[] ?? myChannels.ToArray();

                var teams = await this.GetTeams();
                var teamsList = teams as Team[] ?? teams.ToArray();
                var notMyTeams = teamsList.Except(myTeams, new TeamComparer()).ToList();

                // Do something with the teams not in 'myTeams'
                foreach (var team in notMyTeams.Where(x => x.AllowOpenInvite is true))
                {
                    await this.JoinTeam(this.UserId, team.Id);
                }

                foreach (var team in teamsList)
                {
                    _log.Trace($"{this.UserId} TEAM {team.Name}");
                    var channels = await this.GetChannelsByTeam(team.Id);
                    var channelsList = channels as Channel[] ?? channels.ToArray();
                    var notMyChannels = channelsList.Except(myChannelsList, new ChannelComparer()).ToList();

                    foreach (var channel in notMyChannels)
                    {
                        await this.JoinChannel(this.UserId, channel.Id);
                    }

                    foreach (var channel in channelsList)
                    {
                        _log.Trace($"{this.UserId} CHANNEL: {channel.Id}, {channel.Name}");
                        var lastPost = channelHistory.OrderByDescending(x => x.Created)
                            .FirstOrDefault(x => x.ChannelId == channel.Id);
                        var postId = string.Empty;
                        if (lastPost != null)
                            postId = lastPost.PostId;
                        var posts = await this.GetPostsByChannel(channel.Id, postId);
                        foreach (var post in posts.Posts.Where(x => x.Value.Type == ""))
                        {
                            var user = await this.GetUserById(post.Value.UserId);
                            if (user == null)
                            {
                                const string email = "some.one@user.com";
                                user = new User
                                    { FirstName = "some", LastName = "one", Email = email, Username = email.CreateUsernameFromEmail() };
                            }

                            channelHistory.Add(new ChannelHistory
                            {
                                ChannelId = channel.Id, ChannelName = channel.Name, UserId = post.Value.Id,
                                PostId = post.Value.Id, 
                                UserName = user.Username,
                                Created = post.Value.CreateAt.ToDateTime(),
                                Message = post.Value.Message
                            });
                        }
                    }
                }

                var feelings = this._configuration.Prompts.GetRandom(random);
                _log.Trace($"{username} looking at posts...");

                if (random.Next(0, 100) > 50)
                {
                    _log.Info($"{username} exiting.");
                    return;
                }

                var randomChannelToPostTo = channelHistory.Select(x => x.ChannelId).ToArray().GetRandom(random);
                var history = channelHistory.Where(x => x.ChannelId == randomChannelToPostTo && x.UserId != me.Username).MaxBy(x => x.Created);

                var historyString = history is { Message.Length: >= 100 } ? history.Message[..100] : history?.Message;

                var prompt = $"Write my update to the chat system that {feelings}";
                var respondingTo = string.Empty;
                if (random.Next(0, 99) > 50 && !string.IsNullOrEmpty(historyString) && history.UserId != me.Id)
                {
                    prompt =
                        $"How do I respond to this? {historyString}";
                    respondingTo = history.UserName;
                }

                var message = await llm.ExecuteQuery(prompt);

                message = message.Clean(this._configuration.Replacements, random);
                if (!string.IsNullOrEmpty(respondingTo))
                {
                    message = $"> {historyString.Replace(">", "")}{Environment.NewLine}{Environment.NewLine}@{respondingTo} {message}";
                }
                
                if (message.ShouldSend(this._configuration.Drops))
                {
                    var post = await this.CreatePost(randomChannelToPostTo, message);
                    _log.Info($"{prompt}|SENT|{post.Message}");
                }
                else
                {
                    _log.Info($"{prompt}|NOT SENT|{message}");
                }

                await Task.Delay(random.Next(5000, 250000));
            }
        }
        catch (Exception e)
        {
            _log.Error($"Chat manage run error! {e}");
        }
    }
}