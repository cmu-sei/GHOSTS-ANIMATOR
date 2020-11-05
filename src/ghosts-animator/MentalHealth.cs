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
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;

namespace Ghosts.Animator
{
    public static class MentalHealth
    {
        public static MentalHealthProfile GetMentalHealth()
        {
            var m = new MentalHealthProfile
            {
                HappyQuotient  = AnimatorRandom.Rand.Next(1,100),
                IQ = GetIQ(),
                MelancholyQuotient  = AnimatorRandom.Rand.Next(1,100),
                SpideySense  = AnimatorRandom.Rand.Next(1,100),
                SenseSomethingIsWrongQuotient  = AnimatorRandom.Rand.Next(1,100),
                AdherenceToPolicy  = AnimatorRandom.Rand.Next(1,100),
                EnthusiasmAndAttitude  = AnimatorRandom.Rand.Next(1,100),
                OpenToFeedback  = AnimatorRandom.Rand.Next(1,100),
                OverallPerformance  = AnimatorRandom.Rand.Next(1,100),
                GeneralPerformance  = AnimatorRandom.Rand.Next(1,100),
                InterpersonalSkills = AnimatorRandom.Rand.Next(1,100)
            };
            return m;
        }

        public static int GetIQ()
        {
            return new Dictionary<string, double>
            {
                {"130|210", 2},
                {"121|130", 6},
                {"111|120", 16},
                {"90|110", 52},
                {"80|89", 15},
                {"70|79", 7}
            }.RandomFromPipedProbabilityList();
        }
    }
}