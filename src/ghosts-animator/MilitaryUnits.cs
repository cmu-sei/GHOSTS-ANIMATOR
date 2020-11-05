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
using System.IO;
using System.Linq;
using Ghosts.Animator.Enums;
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;
using Ghosts.Animator.Services;
using Newtonsoft.Json;

namespace Ghosts.Animator
{
    public static class MilitaryUnits
    {
        public static Enums.MilitaryBranch GetServiceBranch()
        {
            var o = Enum.GetValues(typeof(MilitaryBranch)).Cast<MilitaryBranch>().ToList();
            return o.RandomElement();
        }
        
        public static MilitaryUnit GetAll()
        {
            var o = GetAllEx();
            return o;
        }

        public static IEnumerable<MilitaryUnit.Unit> GetAllByServiceBranch(MilitaryBranch branch)
        {
            var o = GetAllEx();
            return o.Sub.Where(x => x.Nick == branch.ToString());
        }

        public static MilitaryUnit GetOneByServiceBranch(MilitaryBranch branch)
        {
            var choice = new MilitaryUnitService(branch, GetAllEx());

            var hq = new MilitaryUnitAddressService(choice.Unit);
            if (!string.IsNullOrEmpty(hq?.MilUnit?.Address?.Name))
            {
                choice.Unit.Address = GetBaseAddress(branch, hq.MilUnit.Address.Name);                
            }
            
            return choice.Unit;
        }

        public static AddressProfiles.AddressProfile GetBaseAddress(MilitaryBranch branch, string hq)
        {
            var a = new AddressProfiles.AddressProfile();

            var raw = File.ReadAllText("config/military_bases.json");
            var o = JsonConvert.DeserializeObject<MilitaryBases.BaseManager>(raw);

            var b = o.Branches.FirstOrDefault(x => x.Name == branch.ToString());
            var myBase = b.Bases.FirstOrDefault(x => x.Name.Equals(hq, StringComparison.InvariantCultureIgnoreCase));
            if (myBase == null)
            {
                myBase = o.Branches.FirstOrDefault(x=>x.Name == branch.ToString())?.Bases.RandomElement();
            }

            if (myBase == null)
                return null;
            
            a.AddressType = "Base";
            a.Name = myBase.Name;
            if (myBase.Streets.Any())
            {
                a.Address1 = myBase.Streets.RandomElement();
            }
            if(string.IsNullOrEmpty(a.Address1))
                a.Address1 = Address.GetStreetAddress();
            a.City = myBase.City;
            a.State = myBase.State;
            a.PostalCode = myBase.PostalCode;
            if (string.IsNullOrEmpty(a.State))
            {
                a.State = Address.GetUSStateAbbreviation();
            }
            if (string.IsNullOrEmpty(a.City))
            {
                var cityAndZip = Address.GetCityAndZipFromStateAbbreviation(a.State);
                a.City = cityAndZip["City"];
                a.PostalCode = cityAndZip["ZipCode"];
            }
            return a;
        }
        
        private static MilitaryUnit GetAllEx()
        {
            var raw = File.ReadAllText("config/military_unit.json");
            var o = JsonConvert.DeserializeObject<MilitaryUnit>(raw);
            return o;
        }
    }
}