// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Enums;
using NUnit.Framework;

namespace Ghosts.Animator.Tests;

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