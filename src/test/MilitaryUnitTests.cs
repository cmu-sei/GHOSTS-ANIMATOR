// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Enums;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class MilitaryUnitTests
{
    [Test]
    public void Get_All_Returns_An_Object()
    {
        var o = MilitaryUnits.GetAll();
            
        ClassicAssert.IsNotNull(o.Sub);
    }
        
    [Test]
    public void Get_By_Branch_Returns_A_Unit()
    {
        var o = MilitaryUnits.GetAllByServiceBranch(MilitaryBranch.USMC);
        foreach (var unit in o)
        {
            ClassicAssert.IsNotEmpty(unit.Name);
        }
    }
        
    [Test]
    public void Base_Address_Is_A_Valid_Address()
    {
        var o = MilitaryUnits.GetBaseAddress(MilitaryBranch.USMC , "Camp Pendleton");
        ClassicAssert.IsNotEmpty(o.Address1);
        ClassicAssert.IsNotEmpty(o.City);
        ClassicAssert.IsNotEmpty(o.State);
        ClassicAssert.IsNotEmpty(o.PostalCode);
    }
        
    [Test]
    public void Service_Branch_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(MilitaryUnits.GetServiceBranch());
    }
}