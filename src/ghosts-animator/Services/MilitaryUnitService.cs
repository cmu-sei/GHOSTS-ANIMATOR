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
using System.Linq;
using Ghosts.Animator.Enums;
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;

namespace Ghosts.Animator.Services
{
    public class MilitaryUnitService
    {
        public MilitaryUnit Unit { get; set; }

        public MilitaryUnitService(MilitaryBranch branch, MilitaryUnit units)
        {
            var unit = units.Sub.ToList().FirstOrDefault(x => x.Nick == branch.ToString());

            this.Unit = units.Clone();
            this.Unit.Sub = null;

            if (unit != null)
            {
                var currentUnit = unit.Clone();
                var list = new List<MilitaryUnit.Unit>();

                while (currentUnit != null)
                {
                    if (currentUnit.Sub == null)
                    {
                        break;
                    }

                    currentUnit = GetUnit(currentUnit);
                    list.Add(currentUnit);
                }

                list.Reverse();
                List<MilitaryUnit.Unit> previous = null;
                foreach (var item in list)
                {
                    if (previous == null)
                    {
                        previous = new List<MilitaryUnit.Unit>();
                    }

                    previous = SetUnit(previous, new List<MilitaryUnit.Unit> {item});
                }

                this.Unit.Sub = previous;
            }

            this.Unit.Country = units.Country;
        }

        private static List<MilitaryUnit.Unit> SetUnit(List<MilitaryUnit.Unit> previous, List<MilitaryUnit.Unit> current)
        {
            foreach (var item in current)
            {
                item.Sub = previous;
            }

            return new List<MilitaryUnit.Unit> {current[0]};
        }

        private static MilitaryUnit.Unit GetUnit(MilitaryUnit.Unit unit)
        {
            return unit.Sub == null ? null : !unit.Sub.Any() ? null : unit.Sub.ToList().RandomElement().Clone();
        }
    }

    public class MilitaryUnitAddressService
    {
        public MilitaryUnit MilUnit { get; private set; }

        public MilitaryUnitAddressService(MilitaryUnit militaryUnit)
        {
            MilUnit = militaryUnit;

            var currentUnit = militaryUnit.Sub.First();

            while (currentUnit != null)
            {
                if (currentUnit.Sub == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(currentUnit.HQ))
                {
                    MilUnit.Address = new AddressProfiles.AddressProfile();
                    MilUnit.Address.Name = currentUnit.HQ;
                }

                currentUnit = GetUnit(currentUnit);
            }
        }

        private static MilitaryUnit.Unit GetUnit(MilitaryUnit.Unit unit)
        {
            return unit.Sub == null ? null : !unit.Sub.Any() ? null : unit.Sub.First();
        }
    }
}