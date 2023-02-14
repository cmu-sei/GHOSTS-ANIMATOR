// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Social.Animations;

public class SocialBeliefJob
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IMongoCollection<NPC> _mongo;
    private List<SocialGraph> _socialGraphs;
    private readonly Random _random;
    private bool isEnabled = true;

    private const string SavePath = "output/socialbelief/";
    private const string SocialGraphFile = "social_belief.json";

    public static string GetSocialGraphFile()
    {
        return SavePath + SocialGraphFile;
    }

    public SocialBeliefJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random)
    {
        try
        {
            this._configuration = configuration;
            this._random = random;
            this._mongo = mongo;

            this.LoadSocialBeliefs();

            if (this._socialGraphs.Count > 0 && this._socialGraphs[0].CurrentStep > _configuration.Animations.SocialGraph.MaximumSteps)
            {
                _log.Trace("SocialBelief has exceed maximum steps. Sleeping...");
                return;
            }

            _log.Info("SocialBelief loaded, running steps...");
            while (this.isEnabled)
            {
                foreach (var graph in this._socialGraphs)
                {
                    this.Step(graph);
                }

                // post-step activities: saving results and reporting on them
                File.WriteAllText(GetSocialGraphFile(), JsonConvert.SerializeObject(this._socialGraphs, Formatting.None));
                this.Report();
                _log.Info($"Step complete, sleeping for {this._configuration.Animations.SocialGraph.TurnLength}ms");
                Thread.Sleep(this._configuration.Animations.SocialBelief.TurnLength);
            }
        }
        catch (ThreadInterruptedException)
        {
            // continue
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void LoadSocialBeliefs()
    {
        var graphs = new List<SocialGraph>();
        var path = SavePath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += SocialGraphFile;
        if (File.Exists(path))
        {
            graphs = JsonConvert.DeserializeObject<List<SocialGraph>>(File.ReadAllText(path));
            _log.Info("SocialBelief loaded from disk...");
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
                foreach (var connection in from sub in list
                         where sub.Id != item.Id
                         select new SocialGraph.SocialConnection
                         {
                             Id = sub.Id,
                             Name = sub.Name.ToString()
                         })
                {
                    graph.Connections.Add(connection);
                }

                graphs.Add(graph);
            }

            _log.Info("SocialBelief created from DB...");
        }

        this._socialGraphs = graphs;
    }

    private void Step(SocialGraph graph)
    {
        if (graph.CurrentStep > this._configuration.Animations.SocialBelief.MaximumSteps)
        {
            _log.Trace($"Maximum steps met: {graph.CurrentStep - 1}. SocialBelief is exiting...");
            this.isEnabled = false;
            return;
        }

        graph.CurrentStep++;

        SocialGraph.Belief belief = null;

        if (graph.Beliefs != null)
        {
            belief = graph.Beliefs.MaxBy(x => x.Step);
        }
        else
        {
            graph.Beliefs = new List<SocialGraph.Belief>();
        }

        if (belief == null)
        {
            var l = Convert.ToDecimal(this._random.NextDouble() * (0.75 - 0.25) + 0.25);
            
            belief = new SocialGraph.Belief(graph.Id, graph.Id, "H_1", graph.CurrentStep, l, (decimal)0.5);
        }

        var bayes = new Bayes(graph.CurrentStep, belief.Likelihood, belief.Posterior, 1 - belief.Likelihood, 1 - belief.Posterior);
        var newBelief = new SocialGraph.Belief(graph.Id, graph.Id, "H_1", graph.CurrentStep, belief.Likelihood, bayes.PosteriorH1);
        graph.Beliefs.Add(newBelief);
    }


    private void Report()
    {
        var line = new StringBuilder();

        //write header
        line.Append(SocialGraph.Belief.ToHeader())
            .Append(Environment.NewLine);

        //now write each person
        foreach (var npc in this._socialGraphs)
        {
            line.Append(npc.Id).Append(',').Append(npc.Name).Append(",H_1,,,")
                .Append(Environment.NewLine);

            foreach (var belief in npc.Beliefs)
            {
                line.Append(",,,").Append(belief.Step).Append(',').Append(belief.Likelihood).Append(',').Append(belief.Posterior)
                    .Append(Environment.NewLine);
            }
        }

        File.WriteAllText($"{SavePath}social_beliefs.csv", line.ToString().TrimEnd(',') + Environment.NewLine);
    }
}