// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

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