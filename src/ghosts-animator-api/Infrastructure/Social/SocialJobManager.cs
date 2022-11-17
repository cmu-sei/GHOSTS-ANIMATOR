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
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social.SocialJobs;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Social
{
    public class SocialJobManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ApplicationConfiguration _configuration;
        private IMongoCollection<NPC> _mongo;
        private readonly Random _random;
        
        public SocialJobManager()
        {
            this._configuration = Program.Configuration;
            this._random = Random.Shared;
        }

        public void Run()
        {
            if (!this._configuration.SocialJobs.IsEnabled)
            {
                _log.Info($"Social Jobs are not enabled, skipping.");
                return;
            }

            _log.Info($"Social Jobs are enabled, starting up...");

            var client = new MongoClient(this._configuration.DatabaseSettings.ConnectionString);
            var database = client.GetDatabase(this._configuration.DatabaseSettings.DatabaseName);
            this._mongo = database.GetCollection<NPC>(this._configuration.DatabaseSettings.CollectionNameNPCs);
            
            try
            {
                if (!this._configuration.SocialJobs.SocialGraph.IsEnabled)
                {
                    _log.Info($"Social Graph is not enabled, skipping.");
                }
                else
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        var _ = new SocialGraphJob(this._configuration, this._mongo, this._random);
                    }).Start();
                }
            }
            catch (Exception e)
            {
                _log.Trace(e);
            }
            
            try
            {
                if (!this._configuration.SocialJobs.SocialSharing.IsEnabled)
                {
                    _log.Info($"Social Sharing is not enabled, skipping.");
                }
                else
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        var _ = new SocialSharingJob(this._configuration, this._mongo, this._random);
                    }).Start();
                }
            }
            catch (Exception e)
            {
                _log.Trace(e);
            }
        }
    }
}