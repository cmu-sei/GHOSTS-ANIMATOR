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
    public class MilitaryBases
    {
        public class Base
        {
            public string Name { get; set; }
            public IList<string> Streets { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string State { get; set; }
        }

        public class BaseManager
        {
            public string Country { get; set; }
            public IList<Branch> Branches { get; set; }
            
            public class MilitaryBase
            {
                public string Name { get; set; }
                public IList<string> Streets { get; set; }
                public string City { get; set; }
                public string PostalCode { get; set; }
                public string State { get; set; }
            }

            public class Branch
            {
                public string Name { get; set; }
                public IList<MilitaryBase> Bases { get; set; }
            }
        }
    }
}