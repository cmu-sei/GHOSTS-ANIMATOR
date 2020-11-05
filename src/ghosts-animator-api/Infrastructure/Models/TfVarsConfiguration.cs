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

namespace Ghosts.Animator.Api.Infrastructure.Models
{
    public class TfVarsConfiguration
    {
        public string Campaign { get; set; }
        public string Enclave { get; set; }
        public string Team { get; set; }
        public string IpAddressHigh { get; set; }
        public string IpAddressLow { get; set; }
        public string Gateway { get; set; }
        public string Mask { get; set; }

        public IList<string> GetIpPool()
        {
            var pool = new List<string>();

            var lowArr = this.IpAddressLow.Split(".");
            var highArr = this.IpAddressHigh.Split(".");

            var low = Convert.ToInt32(lowArr[lowArr.GetUpperBound(0)]);
            var high = Convert.ToInt32(highArr[highArr.GetUpperBound(0)]);
            
            for (var i = low; i < high; i++)
            {
                pool.Add(ReplaceLastOccurrence(this.IpAddressLow, low.ToString(), i.ToString()));
            }
            pool.Add(this.IpAddressHigh);
            return pool;
        }
        
        private static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            var place = Source.LastIndexOf(Find, StringComparison.CurrentCultureIgnoreCase);

            if(place == -1)
                return Source;

            var result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}