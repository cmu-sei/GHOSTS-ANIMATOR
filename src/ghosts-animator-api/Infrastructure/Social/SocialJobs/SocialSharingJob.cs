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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;
using MongoDB.Driver;
using NLog;
using RestSharp;

namespace Ghosts.Animator.Api.Infrastructure.Social.SocialJobs
{
    public class SocialSharingJob
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ApplicationConfiguration _configuration;
        private readonly IMongoCollection<NPC> _mongo;
        private readonly Random _random;
        private const string SavePath = "output/socialsharing/";
        private int CurrentStep = 0;

        public SocialSharingJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random)
        {
            this._configuration = configuration;
            this._random = random;
            this._mongo = mongo;

            if (_configuration.SocialJobs.SocialSharing.IsInteracting)
            {
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }

                while (true)
                {
                    if (this.CurrentStep > _configuration.SocialJobs.SocialSharing.MaximumSteps)
                    {
                        _log.Trace($"Maximum steps met: {this.CurrentStep - 1}. Social sharing is exiting...");
                        return;
                    }
                    
                    this.Step();
                    Thread.Sleep(this._configuration.SocialJobs.SocialSharing.TurnLength);
                    
                    this.CurrentStep++;
                }
            }
        }

        private void Step()
        {
            //take some random npcs
            var lines = new StringBuilder();
            var agents = this._mongo.Find(x => true).ToList().Shuffle(_random).Take(_random.Next(5, 20));
            foreach (var agent in agents)
            {
                var tweetText = "";
                while (string.IsNullOrEmpty(tweetText))
                {
                    if (agent.Birthdate.Date.DayOfYear == DateTime.Now.Date.DayOfYear)
                    {
                        tweetText = ProcessBirthday(agent);
                    }
                    else
                    {
                        var i = _random.Next(0, 15);
                        tweetText = i switch
                        {
                            0 => ProcessAddress(agent),
                            1 => ProcessFamily(agent),
                            2 => ProcessEmployment(agent),
                            3 => ProcessEducation(agent),
                            4 => ProcessAccount(agent),
                            _ => Faker.Lorem.Sentence() //default is just text, no giveaways
                        };
                    }
                }

                Console.WriteLine($"{agent.Name} said: {tweetText}");
                lines.AppendFormat($"{DateTime.Now}|{agent.Id}|{tweetText}{Environment.NewLine}");

                // var propertyNames = agent.GetType().GetProperties().Select(p => p.Name).ToArray();
                // foreach (var prop in propertyNames)
                // {
                //     var propValue = agent.GetType().GetProperty(prop).GetValue(agent, null);
                //     Console.WriteLine($"{prop}:{propValue}");
                // }
                
                if (_configuration.SocialJobs.SocialSharing.IsSendingTimelinesToGhostsApi && !string.IsNullOrEmpty(_configuration.GhostsApiUrl))
                {
                    //send to ghosts api
                    //_configuration.GhostsApiUrl
                
                    var postPayload = File.ReadAllText(@"config/socializer_post.json");
                    postPayload = postPayload.Replace("{id}", Guid.NewGuid().ToString());
                    postPayload = postPayload.Replace("{user}", agent.Email);
                    postPayload = postPayload.Replace("{message}", tweetText);
                    postPayload = postPayload.Replace("{socializer_url}", _configuration.SocialJobs.SocialSharing.SocializerUrl);
                    postPayload = postPayload.Replace("{now}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    
                    _log.Trace(postPayload);
                    
                    var client = new RestClient(_configuration.GhostsApiUrl);
                    var request = new RestRequest("machineupdates", Method.Post);
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(postPayload);

                    try
                    {
                        var response = client.Execute(request);
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Could not post timeline command to ghosts api {_configuration.GhostsApiUrl}: {e}");
                    }
                }
            }
            
            File.AppendAllText($"{SavePath}tweets.txt", lines.ToString());
        }

        private string ProcessAccount(NpcProfile agent)
        {
            if (!agent.Accounts.Any()) return "";
            {
                var o = agent.Accounts.RandomElement();
                var list = new[]
                {
                    $"Check out my new post on {o.Url}",
                    $"Check out my new picture uploaded to {o.Url}",
                    $"New post on {o.Url}",
                    $"Join my newsletter on {o.Url}"
                };
                return list.RandomFromStringArray();
            }
        }
        
        private string ProcessBirthday(NpcProfile agent)
        {
            var list = new[]
            {
                "Happy birthday to me!", 
                $"Out for dinner on my {DateTime.Now.Year - agent.Birthdate.Year} birthday!",
                $"I'm {DateTime.Now.Year - agent.Birthdate.Year} today!",
                $"{DateTime.Now.Year - agent.Birthdate.Year} looks good on me."
            };
            return list.RandomFromStringArray();
        }

        private string ProcessFamily(NpcProfile agent)
        {
            if (!agent.Family.Members.Any()) return "";
            var o = agent.Family.Members.RandomElement();
            var list = new[]
            {
                $"Visiting my {o.Relationship} {o.Name} today.", 
                $"Hanging with {o.Name} my {o.Relationship}.", 
                $"{o.Relationship} and I say {o.Name} - ",
                $"My {o.Relationship} {o.Name}."
            };
            return $"{list.RandomFromStringArray()} {Faker.Lorem.Sentence()}";
        }

        private string ProcessAddress(NpcProfile agent)
        {
            if (!agent.Address.Any()) return "";
            var o = agent.Address.RandomElement();
            var list = new[]
            {
                $"Visiting the {o.State} capital today. Beautiful!", 
                $"Chilling in {o.City} today. Literally freezing.", 
                $"Love {o.City} - so beautiful!",
                $"Love {o.City} - so great!"
            };
            return list.RandomFromStringArray();
        }

        private string ProcessEmployment(NpcProfile agent)
        {
            if (!agent.Employment.EmploymentRecords.Any()) return "";
            var o = agent.Employment.EmploymentRecords.RandomElement();
            var list = new[] 
            { 
                $"Love working at {o.Company}", 
                $"Hanging with my peeps from {o.Company} at the office today!", 
                $"{o.Company} is hiring for my team - DM me for details",
                $"My team at {o.Company} is hiring WFH - DM me for info",
            };
            return list.RandomFromStringArray();
        }

        private string ProcessEducation(NpcProfile agent)
        {
            if (!agent.Education.Degrees.Any()) return "";
            var o = agent.Education.Degrees.RandomElement();
            var list = new[]
            {
                $"{o.School.Name} is the best school in the world!",
                $"On campus of {o.School.Name} - great to be back!",
                $"The {o.School.Name} campus is beautiful this time of year!",
                $"GO {o.School.Name}!!!"
            };
            return list.RandomFromStringArray();
        }
    }
}