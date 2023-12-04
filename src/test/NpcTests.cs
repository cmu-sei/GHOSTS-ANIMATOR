// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class NpcTests
{
    private readonly NpcProfile _npc = Npc.Generate(MilitaryUnits.GetServiceBranch());

    [Test]
    public void Npc_Is_Not_Null()
    {
        ClassicAssert.NotNull(this._npc);
    }
        
    [Test]
    public void Npc_Has_Unit()
    {
        ClassicAssert.NotNull(this._npc.Unit);
    }
        
    [Test]
    public void Npc_Has_Rank()
    {
        ClassicAssert.NotNull(this._npc.Rank);
        ClassicAssert.IsNotEmpty(this._npc.Rank.MOS);
        ClassicAssert.IsNotEmpty(this._npc.Rank.Billet);
    }
        
    [Test]
    public void Npc_Has_Address()
    {
        ClassicAssert.IsNotEmpty(this._npc.Address[0].PostalCode);
        ClassicAssert.IsNotEmpty(this._npc.Address[0].City);
        ClassicAssert.IsNotEmpty(this._npc.Address[0].State);
    }
}