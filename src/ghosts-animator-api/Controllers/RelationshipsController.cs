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
using System.Linq;
using System.Text;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ghosts.Animator.Api.Controllers
{
    [Controller]
    [Produces("application/json")]
    [Route("/[controller]")]
    public class RelationshipsController : Controller
    {
        private readonly IMongoCollection<NPC> _mongo;
        
        public RelationshipsController(DatabaseSettings.IApplicationDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("profile/{id}")]
        public IActionResult Profile(Guid id)
        {
            var npc = _mongo.Find(x => x.Id == id).FirstOrDefault();
            return View("Profile", npc);
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("files/data1.csv")]
        public FileResult Download()
        {
            const string fileName = "data1.csv";
            var content = new StringBuilder("npc_id,source,target,type").Append(Environment.NewLine);

            var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o=>o.Team);
            
            NPC previousNpc = null;
            var enclave = string.Empty;
            var team = string.Empty;
            var campaign = string.Empty;
            
            foreach (var npc in list)
            {
                if (previousNpc == null)
                {
                    campaign = npc.Campaign;
                }
                
                if (previousNpc == null || previousNpc?.Enclave != npc.Enclave)
                {
                    enclave = npc.Enclave;
                    content.Append(",").Append(campaign).Append(",").Append(enclave).Append(",CAMPAIGN").Append(Environment.NewLine);
                }
                
                if (string.IsNullOrEmpty(team) || previousNpc?.Team != npc.Team)
                {
                    team = $"{enclave}/{npc.Team}";
                    content.Append(",").Append(enclave).Append(",").Append(team).Append(",TEAM").Append(Environment.NewLine);
                }

                content.Append(npc.Id).Append(",").Append(team).Append(",").Append(npc.Name).Append(",").Append(npc.Enclave).Append("-").Append(npc.Team).Append(Environment.NewLine);
                previousNpc = npc;
            }
            var fileBytes = Encoding.ASCII.GetBytes(content.ToString());
            return File(fileBytes, "text/csv", fileName);
        }
    }
}