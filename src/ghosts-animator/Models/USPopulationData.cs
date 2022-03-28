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
    public class USPopulationData
    {
        public int TotalPopulation { get; set; }
        public IList<State> States { get; set; }
        
        public class State
        {
            public string Name { get; set; }
            public int Population { get; set; }
            public string Abbreviation { get; set; }
            public IList<City> Cities { get; set; }
            
            public class City
            {
                public string Name { get; set; }
                public string County { get; set; }
                public string Timezone { get; set; }
                public int Population { get; set; }
                public IList<ZipCode> ZipCodes { get; set; }
                
                public class ZipCode
                {
                    public string Id { get; set; }
                    public int Population { get; set; }
                }
            }
        }
    }
}