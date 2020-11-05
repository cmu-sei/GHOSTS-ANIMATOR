using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace Ghosts.Animator.Api.Infrastructure.Models
{
    public class EnclaveReducedCsv
    {
        public string CsvData { get; set; }

        public EnclaveReducedCsv(string[] fieldsToReturn, Dictionary<string, Dictionary<string, string>> npcDictionary)
        {
            var rowList = new List<string>();
            var fields = String.Join(",", fieldsToReturn);
            var header = "Name," + fields;
            rowList.Add(header);

            
            foreach (var npc in npcDictionary)
            {
                var npcRow = new List<string>() {npc.Key};
                foreach (var property in fieldsToReturn)
                {
                    var val = npcDictionary[npc.Key].ContainsKey(property) ? npcDictionary[npc.Key][property] : "";
                    npcRow.Add(val);
                }
                rowList.Add(String.Join(",", npcRow));
            }

            CsvData = String.Join(System.Environment.NewLine, rowList);
            
        }
    }
}