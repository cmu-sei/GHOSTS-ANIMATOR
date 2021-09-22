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
using Ghosts.Animator.Models;
using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class NpcTests
    {
        private NpcProfile _npc;
        public NpcTests()
        {
            this._npc = Npc.Generate(MilitaryUnits.GetServiceBranch());
        }
        
        [Test]
        public void Npc_Is_Not_Null()
        {
            Assert.NotNull(this._npc);
        }
        
        [Test]
        public void Npc_Has_Unit()
        {
            Assert.NotNull(this._npc.Unit);
        }
        
        [Test]
        public void Npc_Has_Rank()
        {
            Assert.NotNull(this._npc.Rank);
            Assert.IsNotEmpty(this._npc.Rank.MOS);
            Assert.IsNotEmpty(this._npc.Rank.Billet);
        }
        
        [Test]
        public void Npc_Has_Address()
        {
            Assert.IsNotEmpty(this._npc.Address[0].PostalCode);
            Assert.IsNotEmpty(this._npc.Address[0].City);
            Assert.IsNotEmpty(this._npc.Address[0].State);
        }
    }
}