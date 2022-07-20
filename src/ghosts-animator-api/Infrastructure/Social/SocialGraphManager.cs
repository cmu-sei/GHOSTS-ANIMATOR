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
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Social
{
    
        public class SocialGraphManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ApplicationConfiguration _configuration;
        private IMongoCollection<NPC> _mongo;
        private List<SocialGraph> _socialGraphs;
        private Random _random;
        private List<Learn> _learns;

        public SocialGraphManager()
        {
            this._configuration = Program.Configuration;
            this._random = new Random();
            this._learns = new List<Learn>();
        }

        public void Run()
        {
            if (!this._configuration.SocialGraph.IsEnabled)
            {
                _log.Error($"SocialGraph is not enabled, skipping.");
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
                
            


                // (a) agents “know” all|many|some|few|none other agents and this affects their interactions, this “know” (relationshipStatus) value changes over time
                // (b) a file -- social_graph.json — gets pushed down to each agent and represents “other agents this agent knows”
                //      it is stored in ./instance, containing id, name, and “know” status of the other agents in the enclave
                // (c) each social_turn (timespan), an interaction may take place, pick n from social_graph.json and interact
                // (d) This interaction may affect knowledge (the existing preferences key/value pair —
                //      the interaction could create a new knowledge, or increase an existing one by some value

                Thread.Sleep(this._configuration.SocialGraph.TurnLength);
            }
        }

        private void LoadSocialGraph()
        {
            var graphs = new List<SocialGraph>();
            var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o=>o.Team).ToList();
            foreach (var item in list)
            {
                //need to build a list of connections for every npc
                var graph = new SocialGraph();
                graph.Id = item.Id;
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

            this._socialGraphs = graphs;
        }

        private void Interact(SocialGraph graph)
        {
            _log.Trace($"{graph.Id} is interacting...");
            
            //get number of agents to interact with, weighted by who is in our graph
            var numberOfAgentsToInteract = 1;//this._random.NextDouble().GetNumberByDecreasingWeights(0);

            //pick other agent(s)
            //IWeightedRandomizer<Guid> randomizer = new DynamicWeightedRandomizer<Guid>();
            // foreach (var item in graph.Connections)
            // {
            //  //   randomizer.Add(item.Id, item.RelationshipStatus);
            //     
            // }

            var agentsToInteract = graph.Connections.RandPick(numberOfAgentsToInteract);
            
            // for (var i = 0; i < numberOfAgentsToInteract; i++)
            // {
            //     
            // }

            //now interact
            foreach (var agent in agentsToInteract)
            {
                //knowledge transferred?
                if (this._random.NextDouble() <= this._configuration.SocialGraph.ChanceOfKnowledgeTransfer)
                {
                    var knowledgeArray = File.ReadAllText("config/knowledge_topics.txt").Split(Environment.NewLine);
                    var learned = knowledgeArray.RandomFromStringArray();
                    var learn = new Learn(graph.Id, agent.Id, learned);
                    _log.Trace($"{graph.Id} gained +1 knowledge of {learned} from {agent.Id}...");
                    this._learns.Add(learn);
                    //new?
                    //update?
                }
                else
                {
                    //_log.Trace($"{graph.Id} didn't learn :\\");
                }
                
                
                //does relationship value change?
                foreach (var learn in this._learns)
                {
                    if (this._learns.Count(x => x.To == learn.To && x.From == learn.From) > 2)
                    {
                        _log.Trace($"{graph.Id} and {agent.Id} have learned from each other, so their relationship has improved...");   
                    }
                }
            }
        }
        
        public class Learn
        {
            public Guid To { get; set; }
            public Guid From { get; set; }
            public string Topic { get; set; }

            public Learn(Guid to, Guid from, string topic)
            {
                this.From = from;
                this.To = to;
                this.Topic = topic;
            }

            public override string ToString()
            {
                return $"{this.To} gained +1 knowledge of {this.Topic} from {this.From}...";
            }
        }
    }
}