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
using System.Text;

namespace Ghosts.Animator.Models
{
    class MilitaryHeightWeight
    {
        public class Heights
        {
            public int Height { get; set; }
            public int MinWeight { get; set; }
            public int MaxWeight { get; set; }
        }

        public class Ages
        {
            public int Age { get; set; }
            public IList<Heights> Heights { get; set; } //ARMY Heights list found here, instead of in Sexes
        }

        public class Sexes
        {
            public string Sex { get; set; } //Will be Null for AF and CG
            public IList<Heights> Heights { get; set; } //Will be Null for ARMY (I think, warrants testing)
            public IList<Ages> Ages { get; set; } //Will be Null for all branches except ARMY
        }

        public class Branches
        {
            public string Branch { get; set; }
            public IList<Heights> Heights { get; set; }
            public IList<Sexes> Sexes { get; set; } //Will be Null for AF and CG
        }

        public class MilitaryHeightWeightManager
        {
            public IList<Branches> Branches { get; set; }
        }
    }
}
