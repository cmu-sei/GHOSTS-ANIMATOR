/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
using Weighted_Randomizer;

namespace Ghosts.Animator.Api.Infrastructure.Social
{
    public class SocialGraphManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ApplicationConfiguration _configuration;
        private IMongoCollection<NPC> _mongo;
        private List<SocialGraph> _socialGraphs;
        private readonly Random _random;
        private string[] _knowledgeArray; 

        public SocialGraphManager()
        {
            this._configuration = Program.Configuration;
            this._random = Random.Shared;
            this._knowledgeArray = GetKnowledge();
        }

        public void Run()
        {
            if (!this._configuration.SocialGraph.IsEnabled)
            {
                _log.Info($"SocialGraph is not enabled, skipping.");
                return;
            }

            _log.Info($"SocialGraph is enabled, starting up...");

            var client = new MongoClient(this._configuration.DatabaseSettings.ConnectionString);
            var database = client.GetDatabase(this._configuration.DatabaseSettings.DatabaseName);
            this._mongo = database.GetCollection<NPC>(this._configuration.DatabaseSettings.CollectionNameNPCs);

            try
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    RunEx();
                }).Start();
            }
            catch (Exception e)
            {
                _log.Trace(e);
            }
        }

        private void RunEx()
        {
            this.LoadSocialGraph();
            while (true)
            {
                foreach (var graph in this._socialGraphs)
                {
                    this.Step(graph);
                }

                File.WriteAllText("output/social_graph.json", JsonConvert.SerializeObject(this._socialGraphs, Formatting.Indented));

                this.ReportMatrix();
                this.ReportLearning();

                _log.Info($"Step complete, sleeping for {this._configuration.SocialGraph.TurnLength}ms");
                Thread.Sleep(this._configuration.SocialGraph.TurnLength);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void LoadSocialGraph()
        {
            var graphs = new List<SocialGraph>();
            var path = "output/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "social_graph.json";
            if (File.Exists(path))
            {
                graphs = JsonConvert.DeserializeObject<List<SocialGraph>>(File.ReadAllText(path));
            }
            else
            {
                var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o => o.Team).Take(25).ToList();
                foreach (var item in list)
                {
                    //need to build a list of connections for every npc
                    var graph = new SocialGraph
                    {
                        Id = item.Id,
                        Name = item.Name.ToString()
                    };
                    foreach (var sub in list)
                    {
                        if (sub.Id == item.Id) continue;
                        var connection = new SocialGraph.SocialConnection
                        {
                            Id = sub.Id,
                            Name = sub.Name.ToString()
                        };
                        graph.Connections.Add(connection);
                    }

                    graphs.Add(graph);
                }
            }

            this._socialGraphs = graphs;
        }

        // (a) agents “know” all|many|some|few|none other agents and this affects their interactions, this “know” (relationshipStatus) value changes over time
        // (b) a file -- social_graph.json — gets pushed down to each agent and represents “other agents this agent knows”
        //      it is stored in ./instance, containing id, name, and “know” status of the other agents in the enclave
        // (c) each social_turn (timespan), an interaction may take place, pick n from social_graph.json and interact
        // (d) This interaction may affect knowledge (the existing preferences key/value pair —
        //      the interaction could create a new knowledge, or increase an existing one by some value
        private void Step(SocialGraph graph)
        {
            graph.CurrentStep++;
            _log.Trace($"{graph.Id} is interacting at step {graph.CurrentStep}...");

            //get number of agents to interact with, weighted by decreasing weights calculation and who is in our graph
            var numberOfAgentsToInteract = this._random.NextDouble().GetNumberByDecreasingWeights(0, graph.Connections.Count, 0.4);

            //pick other agent(s)
            //allowing multiple interactions with the same person because that seems likely
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
                    _log.Trace(learning.ToString);

                    //does relationship value change?
                    if (CalculateRelationshipChange(graph, learning))
                    {
                        var connection = graph.Connections.FirstOrDefault(x => x.Id == learning.From);
                        if (connection is not null)
                        {
                            connection.RelationshipStatus++;
                            _log.Trace($"{learning.To}:{learning.From} Relationship improved...");
                        }
                    }
                }
                else
                {
                    _log.Trace($"{graph.Id} didn't learn...");
                }
            }
            
            //decay
            //https://pubsonline.informs.org/doi/10.1287/orsc.1090.0468
            //in knowledge - the world changes, and so there is some amount of knowledge that is now obsolete
            //in relationships - without work, relationships just sort of evaporate
            var stepsToDecay = this._configuration.SocialGraph.Decay.StepsTo;
            if (graph.CurrentStep > stepsToDecay && this._configuration.SocialGraph.Decay.ChanceOf.BeatsDiceRoll())
            {
                foreach (var k in graph.Knowledge.ToList().DistinctBy(x=>x.Topic))
                {
                    if (graph.Knowledge.Where(x => x.Topic == k.Topic).MaxBy(x => x.Step)?.Step < graph.CurrentStep - stepsToDecay)
                    {
                        var learning = new SocialGraph.Learning(graph.Id, graph.Id, k.Topic, graph.CurrentStep, -1);
                        graph.Knowledge.Add(learning);
                    }
                }

                foreach (var c in graph.Connections)
                {
                    if(c.Interactions.Count > 1 && c.Interactions.MaxBy(x=>x.Step)?.Step < graph.CurrentStep - stepsToDecay)
                    {
                        var interaction = new SocialGraph.Interaction
                        {
                            Step = graph.CurrentStep,
                            Value = -1
                        };
                        c.Interactions.Add(interaction);
                    }
                }
            }
        }

        private string CalculateLearning(SocialGraph graph)
        {
            string knowledge = null;

            var chance = this._configuration.SocialGraph.ChanceOfKnowledgeTransfer;
            var npc = this._mongo.Find(x => x.Id == graph.Id).FirstOrDefault();
            chance += (npc.Education.Degrees.Count * .1);
            chance += (npc.MentalHealth.OverallPerformance * .1);
            chance += (graph.Knowledge.Count * .1);
            
            if (chance.BeatsDiceRoll())
            {
                //knowledge is probably weighted to what you already know
                var randomizer = new DynamicWeightedRandomizer<string>();
                foreach (var k in this._knowledgeArray)
                {
                    if(!randomizer.Contains(k))
                        randomizer.Add(k, graph.Knowledge.Count(x => x.Topic == k) + 1);
                }

                knowledge = randomizer.NextWithReplacement();
            }

            return knowledge;
        }

        private bool CalculateRelationshipChange(SocialGraph graph, SocialGraph.Learning learning)
        {
            return graph.Knowledge.Count(x => x.From == learning.From) > 1;
        }

        private string[] GetKnowledge()
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

            File.WriteAllText($"output/social_knowledge.csv", line.ToString().TrimEnd(',') + Environment.NewLine);

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
            
            File.WriteAllText($"output/social_knowledge_graph.csv", line.ToString().TrimEnd(',') + Environment.NewLine);
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

            File.WriteAllText($"output/social_matrix.csv", line.ToString().TrimEnd(',') + Environment.NewLine);
        }
    }
}