// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class FamilyTests
{
    [Test]
    public void Members_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Family.GetMembers());
    }
        
    [Test]
    public void Member_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Family.GetMember());
    }
        
    [Test]
    public void Relationship_Is_A_Non_Empty_String()
    {
        ClassicAssert.IsNotEmpty(Family.GetRelationship());
    }
}