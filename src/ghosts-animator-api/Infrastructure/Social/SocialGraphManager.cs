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

namespace Ghosts.Animator.Api.Infrastructure.Social
{
    public class SocialGraphManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ApplicationConfiguration _configuration;
        private IMongoCollection<NPC> _mongo;
        private List<SocialGraph> _socialGraphs;
        private readonly Random _random;

        public SocialGraphManager()
        {
            this._configuration = Program.Configuration;
            this._random = new Random();
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
                    this.Interact(graph);
                }

                File.WriteAllText("output/social_graph.json", JsonConvert.SerializeObject(this._socialGraphs, Formatting.Indented));

                this.Matrix();
                this.Learning();

                Thread.Sleep(this._configuration.SocialGraph.TurnLength);
            }
        }

        private void LoadSocialGraph()
        {
            var graphs = new List<SocialGraph>();
            var path = "output/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = path + "social_graph.json";
            if (File.Exists(path))
            {
                var serializer = new JsonSerializer();
                graphs = JsonConvert.DeserializeObject<List<SocialGraph>>(File.ReadAllText(path));
            }
            else
            {
                var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o => o.Team).Take(25).ToList();
                foreach (var item in list)
                {
                    //need to build a list of connections for every npc
                    var graph = new SocialGraph();
                    graph.Id = item.Id;
                    graph.Name = item.Name.ToString();
                    foreach (var sub in list)
                    {
                        if (sub.Id == item.Id) continue;
                        var connection = new SocialGraph.SocialConnection();
                        connection.Id = sub.Id;
                        connection.Name = sub.Name.ToString();
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
        private void Interact(SocialGraph graph)
        {
            //_log.Trace($"{graph.Id} is interacting...");
            graph.CurrentStep++;

            //get number of agents to interact with, weighted by who is in our graph
            //TODO: this should be a decreasing weights calculation
            var numberOfAgentsToInteract = 1; //this._random.NextDouble().GetNumberByDecreasingWeights(0);

            //pick other agent(s)
            //TODO: make this weighted by distance and/or influenced by like knowledge
            var agentsToInteract = graph.Connections.RandPick(numberOfAgentsToInteract);

            // for (var i = 0; i < numberOfAgentsToInteract; i++)
            // {
            //     
            // }

            var knowledgeArray = File.ReadAllText("config/knowledge_topics.txt").Split(Environment.NewLine);

            //now interact
            foreach (var agent in agentsToInteract)
            {
                //knowledge transferred?
                if (this._random.NextDouble() <= this._configuration.SocialGraph.ChanceOfKnowledgeTransfer)
                {
                    var topic = knowledgeArray.RandomFromStringArray();
                    var learning = new SocialGraph.Learning(graph.Id, agent.Id, topic, graph.CurrentStep);
                    graph.Knowledge.Add(learning);
                    _log.Trace(learning.ToString);

                    //does relationship value change?
                    if (graph.Knowledge.Count(x => x.From == learning.From) > 1)
                    {
                        var connection = graph.Connections.FirstOrDefault(x => x.Id == learning.From);
                        if (connection != null)
                        {
                            connection.RelationshipStatus++;
                            _log.Trace($"{learning.To}:{learning.From} Relationship improved");
                        }
                    }
                }
                else
                {
                    //_log.Trace($"{graph.Id} didn't learn :\\");
                }
            }
        }

        private void Learning()
        {
            var line = new StringBuilder(",");

            var knowledgeArray = File.ReadAllText("config/knowledge_topics.txt").Split(Environment.NewLine);
            Array.Sort(knowledgeArray);

            //write header
            foreach (var knowledge in knowledgeArray)
            {
                line.Append(knowledge).Append(',');
            }

            line.Append(Environment.NewLine);


            //now write each person
            foreach (var npc in this._socialGraphs)
            {
                line.Append(npc.Name).Append(',');

                foreach (var knowledge in knowledgeArray)
                {
                    var knowledgeOfTopic = npc.Knowledge.Count(x => x.Topic == knowledge);
                    line.Append(knowledgeOfTopic);
                }

                line.Append(Environment.NewLine);
            }

            File.WriteAllText($"output/social_knowledge.csv", line.ToString().TrimEnd(',') + Environment.NewLine);


            //GRAPH
            //write header
            line = new StringBuilder(",");
            var knowledgeCount = new Dictionary<string, int>();
            foreach (var knowledge in knowledgeArray)
            {
                line.Append(knowledge).Append(',');
                knowledgeCount[knowledge] = 0;
            }

            line.Append(Environment.NewLine);
            //now write each step
            for (long i = 0; i < this._socialGraphs.Max(x => x.CurrentStep); i++)
            {
                foreach (var knowledge in knowledgeArray)
                {
                    foreach (var learningCount in this._socialGraphs.Select(npc => npc.Knowledge.Count(x => x.Topic == knowledge && x.Step < i)))
                    {
                        knowledgeCount[knowledge] += learningCount;
                    }
                }
                line.Append(i).Append(',');
                foreach (var knowledge in knowledgeArray)
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

        private void Matrix()
        {
            var line = new StringBuilder(",");

            //write header
            foreach (var npc in this._socialGraphs)
            {
                line.Append(npc.Name).Append(',');
            }

            line.Append(System.Environment.NewLine);

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

                    var myRelationshipStatus = npc.Connections.FirstOrDefault(x => x.Id == connection.Id).RelationshipStatus;
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