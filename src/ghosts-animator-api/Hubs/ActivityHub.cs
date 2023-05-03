using System;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Hubs;

public class ActivityHub : Hub
{
    internal static Logger _log = LogManager.GetCurrentClassLogger();

    private readonly IMongoCollection<NPC> _mongo;
    private static readonly ConnectionMapping<string> _connections = new();

    public ActivityHub(DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        this._mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
    }

    public override Task OnConnectedAsync()
    {
        _connections.Add("1", this.Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _connections.Remove("1", this.Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Show(int eventId, string npcId, string type, string message, string time)
    {
        _log.Debug("Processing update");

        //do some saving
        
        foreach (var connectionId in _connections.GetConnections("1"))
        {
            var types = new[] { "social", "belief", "knowledge", "relationship" };
            var t = types.RandomFromStringArray();
            
            await Clients.Client(connectionId).SendAsync("show", eventId, 
                npcId, 
                t, 
                Faker.Lorem.Sentence(), 
                DateTime.Now.ToString());
        }
    }
}