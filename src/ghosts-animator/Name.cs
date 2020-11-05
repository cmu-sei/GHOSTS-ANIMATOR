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
using Ghosts.Animator.Extensions;

namespace Ghosts.Animator
{
    public static class Name
    {
        public static Models.NameProfile GetName()
        {
            switch (AnimatorRandom.Rand.Next(4))
            {
                case 0: return new Models.NameProfile {Prefix = GetPrefix(), First = GetFirstName(), Last = GetLastName()};
                case 1: return new Models.NameProfile {First = GetFirstName(), Last = GetLastName(), Suffix = GetSuffix()};
                case 2: return new Models.NameProfile {First = GetFirstName(), Last = GetLastName(), Middle = GetMiddleName()};
                default: return new Models.NameProfile {First = GetFirstName(), Last = GetLastName()};
            }
        }

        public static string GetFirstName()
        {
            var file = $"config/names_{PhysicalCharacteristics.GetBiologicalSex().ToString().ToLower()}.txt";
            if (Npc.NpcProfile != null)
            {
                file = $"config/names_{Npc.NpcProfile.BiologicalSex.ToString().ToLower()}.txt";
            }

            return file.GetRandomFromFile();
        }
        
        public static string GetFirstName(BiologicalSex sex)
        {
            var file = $"config/names_{sex.ToString().ToLower()}.txt";
            return file.GetRandomFromFile();
        }
        
        public static string GetMiddleName()
        {
            return GetFirstName();
        }

        public static string GetLastName()
        {
            return "config/names_last.txt".GetRandomFromFile();
        }

        public static string GetPrefix()
        {
            return PREFIXES.RandomElement();
        }

        public static string GetSuffix()
        {
            return SUFFIXES.RandomElement();
        }

        static readonly string[] PREFIXES = {"Mr.", "Mrs.", "Ms.", "Miss", "Dr."};

        static readonly string[] SUFFIXES = {"Jr.", "Sr.", "I", "II", "III", "IV", "V", "MD", "DDS", "PhD", "DVM"};
    }
}