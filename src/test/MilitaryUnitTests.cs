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
using Ghosts.Animator.Enums;
using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class MilitaryUnitTests
    {
        [Test]
        public void Get_All_Returns_An_Object()
        {
            var o = MilitaryUnits.GetAll();
            
            Assert.IsNotNull(o.Sub);
        }
        
        [Test]
        public void Get_By_Branch_Returns_A_Unit()
        {
            var o = MilitaryUnits.GetAllByServiceBranch(MilitaryBranch.USMC);
            foreach (var unit in o)
            {
                Assert.IsNotEmpty(unit.Name);
            }
        }
        
        [Test]
        public void Base_Address_Is_A_Valid_Address()
        {
            var o = MilitaryUnits.GetBaseAddress(MilitaryBranch.USMC , "Camp Pendleton");
            Assert.IsNotEmpty(o.Address1);
            Assert.IsNotEmpty(o.City);
            Assert.IsNotEmpty(o.State);
            Assert.IsNotEmpty(o.PostalCode);
        }
        
        [Test]
        public void Service_Branch_Is_Not_Null()
        {
            Assert.IsNotNull(MilitaryUnits.GetServiceBranch());
        }
    }
}