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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ghosts.Animator.Api.Infrastructure.Social
{
    public class SocialGraph
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<SocialConnection> Connections { get; set; }
        public IList<Learning> Knowledge { get; set; }
        
        public long CurrentStep { get; set; }
        

        public SocialGraph()
        {
            this.Connections = new List<SocialConnection>();
            this.Knowledge = new List<Learning>();
        }

        public class SocialConnection
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Distance { get; set; }
            public int RelationshipStatus { get; set; }
        }

        public class Learning
        {
            public Guid To { get; set; }
            public Guid From { get; set; }
            public string Topic { get; set; }
            public long Step { get; set; }

            public Learning(Guid to, Guid from, string topic, long currentStep)
            {
                this.From = from;
                this.To = to;
                this.Topic = topic;
                this.Step = currentStep;
            }

            public override string ToString()
            {
                return $"{this.To},{this.From},{this.Topic},{this.Step}";
            }
        }
    }
}