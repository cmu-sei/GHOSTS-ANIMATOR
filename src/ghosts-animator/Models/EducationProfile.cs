/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using Ghosts.Animator.Enums;
using System.Collections.Generic;

namespace Ghosts.Animator.Models
{
    public class EducationProfile
    {
        public class Degree
        {
            public DegreeLevel Level { get; set; }
            public string DegreeType { get; set; }

            public string Major { get; set; }

            //public string Minor;
            //public string Concentration;
            public School School { get; set; }

            public Degree(DegreeLevel level, string major, School school)
            {
                Level = level;
                if (level == DegreeLevel.Bachelors || level == DegreeLevel.Associates
                                                   || level == DegreeLevel.Masters || level == DegreeLevel.Doctorate
                                                   || level == DegreeLevel.Professional)
                {
                    Major = major.Split(',')[0];
                    DegreeType = major.Split(',')[1];
                }
                else
                {
                    Major = major;
                    DegreeType = "";
                }

                School = school;
            }
        }

        public List<Degree> Degrees { get; set; }

        public override string ToString()
        {
            if (Degrees[0].Level == DegreeLevel.None)
            {
                return "Less than High School Education.";
            }

            if (Degrees[0].Level == DegreeLevel.GED)
            {
                return "GED";
            }

            if (Degrees[0].Level == DegreeLevel.HSDiploma)
            {
                return "High School Education.";
            }
            else
            {
                var o = "";
                foreach (var item in Degrees)
                {
                    o += $"{item.DegreeType} in {item.Major} from {item.School.Name}\n";
                }

                return o;
            }
        }
    }
}