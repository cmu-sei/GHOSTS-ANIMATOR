// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
using Weighted_Randomizer;

namespace Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions;

public class SocialGraphJob
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IMongoCollection<NPC> _mongo;
    private List<SocialGraph> _socialGraphs;
    private readonly Random _random;
    private readonly string[] _knowledgeArray;
    private bool _isEnabled = true;

    private const string SavePath = "output/socialgraph/";
    private const string SocialGraphFile = "social_graph.json";
    private readonly IHubContext<ActivityHub> _activityHubContext;
        
    public static string GetSocialGraphFile()
    {
        return SavePath + SocialGraphFile;
    }

    public SocialGraphJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random, IHubContext<ActivityHub> activityHubContext)
    {
        try
        {
            this._activityHubContext = activityHubContext;
            this._configuration = configuration;
            this._random = random;
            this._mongo = mongo;

            this._knowledgeArray = GetKnowledge();
            this.LoadSocialGraphs();

            if (this._socialGraphs.Count > 0 && this._socialGraphs[0].CurrentStep > _configuration.Animations.SocialGraph.MaximumSteps)
            {
                _log.Trace("Graph has exceed maximum steps. Sleeping...");
                return;
            }

            _log.Info("SocialGraph loaded, running steps...");
            while (this._isEnabled)
            {
                foreach (var graph in this._socialGraphs)
                {
                    this.Step(graph);
                }

                // post-step activities: saving results and reporting on them
                File.WriteAllText(GetSocialGraphFile(), JsonConvert.SerializeObject(this._socialGraphs, Formatting.None));
                this.Report();
                _log.Info($"Step complete, sleeping for {this._configuration.Animations.SocialGraph.TurnLength}ms");
                Thread.Sleep(this._configuration.Animations.SocialGraph.TurnLength);
            }
        }
        catch (ThreadInterruptedException)
        {
            // continue
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
    }

    private void LoadSocialGraphs()
    {
        var graphs = new List<SocialGraph>();
        var path = SavePath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += SocialGraphFile;
        if (File.Exists(path))
        {
            graphs = JsonConvert.DeserializeObject<List<SocialGraph>>(File.ReadAllText(path));
            _log.Info("SocialGraph loaded from disk...");
        }
        else
        {
            var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o => o.Team).Take(10).ToList();
            foreach (var item in list)
            {
                //need to build a list of connections for every npc
                var graph = new SocialGraph
                {
                    Id = item.Id,
                    Name = item.Name.ToString()
                };
                foreach (var connection in from sub in list where sub.Id != item.Id select new SocialGraph.SocialConnection
                         {
                             Id = sub.Id,
                             Name = sub.Name.ToString()
                         })
                {
                    graph.Connections.Add(connection);
                }

                graphs.Add(graph);
            }

            _log.Info("SocialGraph created from DB...");
        }

        this._socialGraphs = graphs;
    }

    // (a) agents “know” some number of other agents, affecting their interactions, and this “know” (relationshipStatus) value changes over time
    // (b) a file -- social_graph.json — gets pushed down to each agent and represents “other agents in relation to this agent”
    //      it is stored in ./instance, containing id, name, and “know” status of the other agents in the enclave
    // (c) each step, n interactions may take place, picking from social_graph.json and interacting
    // (d) This interaction may affect knowledge (the existing preferences key/value pair —
    //      the interaction could create new knowledge, or increase an existing one by some value
    private void Step(SocialGraph graph)
    {
        if (graph.CurrentStep > _configuration.Animations.SocialGraph.MaximumSteps)
        {
            _log.Trace($"Maximum steps met: {graph.CurrentStep - 1}. Social graph is exiting...");
            this._isEnabled = false;
            return;
        }

        graph.CurrentStep++;
        _log.Trace($"{graph.CurrentStep}: {graph.Id} is interacting...");

        //get number of agents to interact with, weighted by decreasing weights calculation and who is in our graph
        var numberOfAgentsToInteract = this._random.NextDouble().GetNumberByDecreasingWeights(0, graph.Connections.Count, 0.4);

        //pick other agent(s), allowing multiple interactions with the same person because that seems likely
        //TODO: make this weighted by distance and/or influenced by like knowledge
        var agentsToInteract = graph.Connections.RandPick(numberOfAgentsToInteract);

        //now interact
        foreach (var agent in agentsToInteract)
        {
            var interaction = new SocialGraph.Interaction
            {
                Step = graph.CurrentStep,
                Value = 1
            };
            var interactingWith = graph.Connections.FirstOrDefault(x => x.Id == agent.Id);
            interactingWith?.Interactions.Add(interaction);

            // var npc = _mongo.Find(x => x.Id == graph.Id).FirstOrDefault();
            // npc.MentalHealth.HappyQuotient

            //knowledge transferred?
            var topic = CalculateLearning(graph);
            if (topic is not null)
            {
                var learning = new SocialGraph.Learning(graph.Id, agent.Id, topic, graph.CurrentStep, 1);
                graph.Knowledge.Add(learning);
                
                //post to hub
                this._activityHubContext.Clients.All.SendAsync("show",
                    graph.CurrentStep,
                    agent.Id,
                    "knowledge",
                    $"learned more about {learning.Topic} ({learning.Value})",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture)
                );

                Thread.Sleep(_configuration.Animations.SocialGraph.TurnLength);
                
                _log.Trace(learning.ToString);

                //does relationship value change?
                if (CalculateRelationshipChange(graph, learning))
                {
                    var connection = graph.Connections.FirstOrDefault(x => x.Id == learning.From);
                    if (connection is not null)
                    {
                        connection.RelationshipStatus++;
                        
                        var npcTo = this._mongo.Find(x => x.Id == learning.To).FirstOrDefault();
                        var npcFrom = this._mongo.Find(x => x.Id == learning.From).FirstOrDefault();
                        
                        var o = $"{graph.CurrentStep}: {npcFrom.Name}'s relationship improved with {npcTo.Name}...";
                        _log.Trace(o);
                        
                        //post to hub
                        this._activityHubContext.Clients.All.SendAsync("show",
                            graph.CurrentStep,
                            agent.Id,
                            "relationship",
                            o,
                            DateTime.Now.ToString(CultureInfo.InvariantCulture)
                        );
                    }
                }
            }
            else
            {
                var o = $"{graph.CurrentStep}: {graph.Id} didn't learn...";
                _log.Trace(o);
                
                //post to hub
                this._activityHubContext.Clients.All.SendAsync("show",
                    graph.CurrentStep,
                    agent.Id,
                    "relationship",
                    o,
                    DateTime.Now.ToString(CultureInfo.InvariantCulture)
                );
            }
        }
        
        //decay
        //https://pubsonline.informs.org/doi/10.1287/orsc.1090.0468
        //in knowledge - the world changes, and so there is some amount of knowledge that is now obsolete
        //in relationships - without work, relationships can decay
        var stepsToDecay = this._configuration.Animations.SocialGraph.Decay.StepsTo;
        if (graph.CurrentStep > stepsToDecay)
        {
            foreach (var k in graph.Knowledge.ToList().DistinctBy(x => x.Topic))
            {
                if (graph.Knowledge.Where(x => x.Topic == k.Topic).Sum(x => x.Value) > stepsToDecay)
                {
                    if (this._configuration.Animations.SocialGraph.Decay.ChanceOf.ChanceOfThisValue())
                    {
                        if (graph.Knowledge.Where(x => x.Topic == k.Topic).MaxBy(x => x.Step)?.Step < graph.CurrentStep - stepsToDecay)
                        {
                            var learning = new SocialGraph.Learning(graph.Id, graph.Id, k.Topic, graph.CurrentStep, -1);
                            graph.Knowledge.Add(learning);
                            
                            //post to hub
                            this._activityHubContext.Clients.All.SendAsync("show",
                                graph.CurrentStep,
                                graph.Id,
                                "knowledge",
                                $"had knowledge decay in {k.Topic} occur",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture)
                            );
                            
                            break;
                        }
                    }
                }
            }

            foreach (var c in graph.Connections)
            {
                if (c.Interactions.Count > 1 && c.Interactions.MaxBy(x => x.Step)?.Step < graph.CurrentStep - stepsToDecay)
                {
                    var interaction = new SocialGraph.Interaction
                    {
                        Step = graph.CurrentStep,
                        Value = -1
                    };
                    c.Interactions.Add(interaction);
                    
                    //post to hub
                    this._activityHubContext.Clients.All.SendAsync("show",
                        graph.CurrentStep,
                        graph.Id,
                        "relationship",
                        $"Experienced general relationship decay",
                        DateTime.Now.ToString(CultureInfo.InvariantCulture)
                    );
                }
            }
        }
    }

    private string CalculateLearning(SocialGraph graph)
    {
        string knowledge = null;

        var chance = this._configuration.Animations.SocialGraph.ChanceOfKnowledgeTransfer;
        var npc = this._mongo.Find(x => x.Id == graph.Id).FirstOrDefault();
        if (npc == null)
            return string.Empty;
        
        chance += npc.Education.Degrees.Count * .1;
        chance += npc.MentalHealth.OverallPerformance * .1;
        chance += graph.Knowledge.Count * .1;

        if (chance.ChanceOfThisValue())
        {
            //knowledge is probably weighted to what you already know
            var randomizer = new DynamicWeightedRandomizer<string>();
            foreach (var k in this._knowledgeArray)
            {
                if (!randomizer.Contains(k))
                    randomizer.Add(k, graph.Knowledge.Count(x => x.Topic == k) + 1);
            }

            knowledge = randomizer.NextWithReplacement();
        }

        return knowledge;
    }

    private static bool CalculateRelationshipChange(SocialGraph graph, SocialGraph.Learning learning)
    {
        return graph.Knowledge.Count(x => x.From == learning.From) > 1;
    }

    private static string[] GetKnowledge()
    {
        var k = File.ReadAllText("config/knowledge_topics.txt").Split(Environment.NewLine);
        Array.Sort(k);
        return k;
    }

    private void ReportLearning()
    {
        var line = new StringBuilder(",");

        //write header
        foreach (var knowledge in this._knowledgeArray)
        {
            line.Append(knowledge).Append(',');
        }

        line.Append(Environment.NewLine);

        //now write each person
        foreach (var npc in this._socialGraphs)
        {
            line.Append(npc.Name).Append(',');

            foreach (var knowledge in this._knowledgeArray)
            {
                var knowledgeOfTopic = npc.Knowledge.Count(x => x.Topic == knowledge);
                line.Append(knowledgeOfTopic).Append(',');
            }

            line.Append(Environment.NewLine);
        }

        File.WriteAllText($"{SavePath}social_knowledge.csv", line.ToString().TrimEnd(',') + Environment.NewLine);

        //GRAPH
        //write header
        line = new StringBuilder(",");
        var knowledgeCount = new Dictionary<string, int>();
        foreach (var knowledge in this._knowledgeArray)
        {
            line.Append(knowledge).Append(',');
            knowledgeCount[knowledge] = 0;
        }

        line.Append(Environment.NewLine);
        //now write each step
        for (long i = 0; i < this._socialGraphs.Max(x => x.CurrentStep); i++)
        {
            foreach (var knowledge in this._knowledgeArray)
            {
                // ReSharper disable once AccessToModifiedClosure
                foreach (var learningCount in this._socialGraphs.Select(npc => npc.Knowledge.Count(x => x.Topic == knowledge && x.Step < i)))
                {
                    knowledgeCount[knowledge] += learningCount;
                }
            }

            line.Append(i).Append(',');
            foreach (var knowledge in this._knowledgeArray)
            {
                line.Append(knowledgeCount[knowledge]).Append(',');
            }

            line.Length--;
            line.Append(Environment.NewLine);

            foreach (var key in knowledgeCount.Keys)
            {
                knowledgeCount[key] = 0;
            }
        }

        File.WriteAllText($"{SavePath}social_knowledge_graph.csv", line.ToString().TrimEnd(',') + Environment.NewLine);
    }

    private void Report()
    {
        this.ReportMatrix();
        this.ReportLearning();
    }

    private void ReportMatrix()
    {
        var line = new StringBuilder(",");

        //write header
        foreach (var npc in this._socialGraphs)
        {
            line.Append(npc.Name).Append(',');
        }

        line.Append(Environment.NewLine);

        //now write each person
        foreach (var npc in this._socialGraphs)
        {
            line.Append(npc.Name).Append(',');
            foreach (var connection in this._socialGraphs)
            {
                if (connection.Id == npc.Id)
                {
                    line.Append(',');
                    continue;
                }

                var myRelationshipStatus = npc.Connections.FirstOrDefault(x => x.Id == connection.Id)!.RelationshipStatus;
                // var otherNpcRelationshipStatus = this._socialGraphs.FirstOrDefault(x => x.Id == connection.Id).Connections
                //     .FirstOrDefault(y => y.Id == npc.Id).RelationshipStatus; 

                //line.Append(myRelationshipStatus).Append("::").Append(otherNpcRelationshipStatus).Append(',');

                line.Append(myRelationshipStatus).Append(',');
            }

            line.Append(Environment.NewLine);
        }

        File.WriteAllText($"{SavePath}social_matrix.csv", line.ToString().TrimEnd(',') + Environment.NewLine);
    }
}