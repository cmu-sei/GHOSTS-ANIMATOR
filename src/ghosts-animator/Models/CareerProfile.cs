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

namespace Ghosts.Animator.Models
{
    public class CareerProfile
    {
        public int WorkEthic { get; set; }
        public int TeamValue { get; set; }
        public IEnumerable<StrengthProfile> Strengths { get; set; }
        public IEnumerable<WeaknessProfile> Weaknesses { get; set; }

        public CareerProfile()
        {
            this.Strengths = new List<StrengthProfile>();
            this.Weaknesses = new List<WeaknessProfile>();
        }

        public class StrengthProfile
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class WeaknessProfile
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}