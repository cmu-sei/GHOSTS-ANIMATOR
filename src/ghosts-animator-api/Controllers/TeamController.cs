// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FileHelpers;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers;

/// <summary>
/// Export or delete all NPCs from a specific team
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    private readonly IMongoCollection<NPC> _mongo;
    private readonly IMongoCollection<NPCIpAddress> _mongoIps;

    public TeamController(DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
        _mongoIps = database.GetCollection<NPCIpAddress>(settings.CollectionNameIPAddresses);
    }

    private static FilterDefinition<NPC> BuildTeamFilter(string campaign, string enclave, string team)
    {
        var filter = Builders<NPC>.Filter.And(
            Builders<NPC>.Filter.Eq("Campaign", campaign),
            Builders<NPC>.Filter.Eq("Enclave", enclave),
            Builders<NPC>.Filter.Eq("Team", team));
        return filter;
    }

    /// <summary>
    /// Gets all NPCs by from a specific team in a specific enclave that is part of a specific campaign
    /// </summary>
    /// <param name="team">The name of the team</param>
    /// <param name="enclave">The name of the enclave the team is in</param>
    /// <param name="campaign">The name of the campaign the enclave is part of</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(IEnumerable<NPC>), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NPC>))]
    [SwaggerOperation("getTeam")]
    [HttpGet("{campaign}/{enclave}/{team}")]
    public IEnumerable<NPC> GetTeam(string campaign, string enclave, string team)
    {
        var filter = BuildTeamFilter(campaign, enclave, team);
        return _mongo.Find(filter).ToList();
    }

    /// <summary>
    /// Gets the csv output of a query
    /// </summary>
    /// <param name="team">The name of the team</param>
    /// <param name="enclave">The name of the enclave the team is in</param>
    /// <param name="campaign">The name of the campaign the enclave is part of</param>
    /// <returns></returns>
    [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IActionResult))]
    [SwaggerOperation("getTeamCsv")]
    [HttpGet("{campaign}/{enclave}/{team}/csv")]
    public IActionResult GetAsCsv(string campaign, string enclave, string team)
    {
        var engine = new FileHelperEngine<NPCToCsv>();
        var list = this.GetTeam(team, enclave, campaign).ToList();

        var filteredList = list.Select(n => new NPCToCsv {Id = n.Id, Email = n.Email}).ToList();

        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        engine.WriteStream(streamWriter, filteredList);
        streamWriter.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "text/csv", $"{Guid.NewGuid()}.csv");
    }

    /// <summary>
    /// Gets the tfvars output of a team of NPCs
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IActionResult))]
    [SwaggerOperation("getBuildTfVars")]
    [HttpPost("tfvars")]
    public IActionResult GetTeamAsTfVars(TfVarsConfiguration configuration)
    {
        var s = new StringBuilder("users = {").Append(Environment.NewLine);
        var list = this.GetTeam(configuration.Campaign, configuration.Enclave, configuration.Team).ToList();
        Console.WriteLine(list.Count);

        var pool = configuration.GetIpPool();
        foreach (var item in pool)
            if (_mongoIps.Find(o => o.IpAddress == item && o.Enclave == configuration.Enclave).Any())
                pool.Remove(item);

        if (pool.Count < list.Count)
        {
            throw new ArgumentException("There are not enough unused ip addresses for the number of NPCs selected");
        }

        var i = 0;
        foreach (var npc in list)
        {
            var n = string.Empty;
            foreach (var c in npc.Finances.CreditCards)
            {
                n = c.Number;
                break;
            }

            var ip = pool.RandomElement();
            _mongoIps.InsertOne(new NPCIpAddress {IpAddress = ip, NpcId = npc.Id, Enclave = npc.Enclave});
            pool.Remove(ip);

            s.Append("\tuser-").Append(i).Append(" = {").Append(Environment.NewLine);
            s.Append("\t\tipaddr = ").Append(ip).Append(Environment.NewLine);
            s.Append("\t\tmask = ").Append(configuration.Mask).Append(Environment.NewLine);
            s.Append("\t\tgateway = ").Append(configuration.Gateway).Append(Environment.NewLine);
            s.Append("\t\ttitle = ").Append(npc.Rank.Abbr).Append(Environment.NewLine);
            s.Append("\t\tfirst = ").Append(npc.Name.First).Append(Environment.NewLine);
            s.Append("\t\tlast = ").Append(npc.Name.Last).Append(Environment.NewLine);
            s.Append("\t\taddress = ").Append(npc.Address[0].Address1).Append(Environment.NewLine);
            s.Append("\t\tcity = ").Append(npc.Address[0].City).Append(Environment.NewLine);
            s.Append("\t\tstate = ").Append(npc.Address[0].State).Append(Environment.NewLine);
            s.Append("\t\tzip = ").Append(npc.Address[0].PostalCode).Append(Environment.NewLine);
            s.Append("\t\temail = ").Append(npc.Email).Append(Environment.NewLine);
            s.Append("\t\tpassword = ").Append(npc.Password).Append(Environment.NewLine);
            s.Append("\t\tcreditcard = ").Append(n).Append(Environment.NewLine);
            s.Append("\t}").Append(Environment.NewLine);
            s.Append(Environment.NewLine);
            i++;
        }

        return File(Encoding.UTF8.GetBytes
            (s.ToString()), "text/tfvars", $"{Guid.NewGuid()}.tfvars");
    }
        
    /// <summary>
    /// Delete All NPCs in a specific team
    /// </summary>
    /// <param name="team"></param>
    /// <param name="enclave"></param>
    /// <param name="campaign"></param>
    /// <returns></returns>
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK)]
    [SwaggerOperation("deleteTeam")]
    [HttpDelete("{campaign}/{enclave}/{team}")]
    public void DeleteTeam(string campaign, string enclave, string team)
    {
        var list = this.GetTeam(campaign, enclave, team).ToList();
        foreach (var ipFilter in list.Select(npc => npc.Id).Select(npcId => Builders<NPCIpAddress>.Filter.And(
                     Builders<NPCIpAddress>.Filter.Eq("NpcId", npcId),
                     Builders<NPCIpAddress>.Filter.Eq("Enclave", enclave))))
        {
            _mongoIps.DeleteMany(ipFilter);
        }

        var npcFilter = BuildTeamFilter(campaign, enclave, team);
        _mongo.DeleteMany(npcFilter);
            
    }
}