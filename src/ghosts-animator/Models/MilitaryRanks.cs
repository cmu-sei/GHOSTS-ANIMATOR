/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System.Collections.Generic;
using Ghosts.Animator.Enums;

namespace Ghosts.Animator.Models
{
    public class MilitaryRank
    {
        public string Country { get; set; }
        public IList<Branch> Branches { get; set; }

        public class Branch
        {
            public string Name { get; set; }
            public IList<Rank> Ranks { get; set; }

            public class Rank
            {
                public MilitaryBranch Branch { get; set; }
                public string Pay { get; set; }
                public string Name { get; set; }
                public string Abbr { get; set; }
                public string Classification { get; set; }
                
                //Position number wrt this unit
                public string Billet { get; set; }
                public string MOS { get; set; }
                public string MOSID { get; set; }
                public double Probability { get; set; }
            }
        }
    }
    
    public class Billet
    {
        public string Pay { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class Branch
    {
        public string Name { get; set; }
        public IList<Billet> Billets { get; set; }
    }

    public class BilletManager
    {
        public string Country { get; set; }
        public IList<Branch> Branches { get; set; }
    }

    public class MOSModels
    {
        public class Item
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
        }

        public class MO
        {
            public string Type { get; set; }
            public IList<Item> Items { get; set; }
            public string Low { get; set; }
        }

        public class Branch
        {
            public string Name { get; set; }
            public IList<MO> MOS { get; set; }
            public string Url { get; set; }
        }

        public class MOSManager
        {
            public string Country { get; set; }
            public IList<Branch> Branches { get; set; }
        }
    }
}