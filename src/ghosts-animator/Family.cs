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
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;

namespace Ghosts.Animator
{
    public static class Family
    {
        public static IEnumerable<FamilyProfile.Person> GetMembers()
        {
            var list = new List<FamilyProfile.Person>();
            for (var i = 0; i < AnimatorRandom.Rand.Next(0, 5); i++)
            {
                list.Add(GetMember());
            }

            return list;
        }

        public static FamilyProfile.Person GetMember()
        {
            if (Npc.NpcProfile != null)
            {
                var name = Name.GetName();
                if (Npc.NpcProfile.BiologicalSex == BiologicalSex.Female)
                    name.First = Name.GetFirstName(BiologicalSex.Male);
                if (Npc.NpcProfile.BiologicalSex == BiologicalSex.Male)
                    name.First = Name.GetFirstName(BiologicalSex.Female);
                return new FamilyProfile.Person {Name = name, Relationship = GetRelationship()};
            }
            return new FamilyProfile.Person {Name = Name.GetName(), Relationship = GetRelationship()};
        }

        public static string GetRelationship()
        {
            return RELATIONSHIPS.RandomElement();
        }

        private static readonly string[] RELATIONSHIPS = {"Spouse", "Parent", "Sibling", "Child"};
    }
}