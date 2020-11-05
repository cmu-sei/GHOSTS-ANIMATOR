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

namespace Ghosts.Animator
{
    public static class AnimatorRandom
    {
        public static Random Rand = new Random();

        public static void Seed(int seed)
        {
            Rand = new Random(seed);
        }

        public static DateTime Date(int yearsAgo = 20)
        {
            var minutesAgo = yearsAgo * 525949;
            return DateTime.Now.AddMinutes(-Rand.Next(30000, minutesAgo)); //make it at least ~ month ago
        }
    }

    public static class PercentOfRandom
    {
        public static bool Does(int percentOfPeopleDo)
        {
            return (AnimatorRandom.Rand.Next(0, 100)) > (100 - percentOfPeopleDo);
        }
    }
}