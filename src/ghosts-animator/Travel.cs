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
using Ghosts.Animator.Models;

namespace Ghosts.Animator
{
    public static class Travel
    {
        public static IEnumerable<ForeignTravelProfile.Trip> GetTrips()
        {
            var list = new List<ForeignTravelProfile.Trip>();
            for (var i = 0; i < AnimatorRandom.Rand.Next(0, 20); i++)
            {
                list.Add(GetTrip());
            }

            return list;
        }

        public static ForeignTravelProfile.Trip GetTrip()
        {
            var arrive = DateTime.Now.AddYears(AnimatorRandom.Rand.Next(-20, -1)).AddHours(AnimatorRandom.Rand.Next(1, 72))
                .AddMinutes(AnimatorRandom.Rand.Next(1, 60)).AddSeconds(AnimatorRandom.Rand.Next(1, 60));
            var depart = arrive.AddDays(AnimatorRandom.Rand.Next(2, 21)).AddHours(AnimatorRandom.Rand.Next(-24, 24))
                .AddMinutes(AnimatorRandom.Rand.Next(-60, 60)).AddSeconds(AnimatorRandom.Rand.Next(-60, 60));

            var address = Address.GetInternationalAddress();
            var code = Address.GetCountryCode(address.Country);
            
            return new ForeignTravelProfile.Trip
                {ArriveDestination = arrive, DepartDestination = depart, Destination = address.ToString(), Country = address.Country, Code = code};
        }
    }
}