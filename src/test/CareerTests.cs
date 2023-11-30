// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using NUnit.Framework;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class CareerTests
{
    [Test]
    public void Strength_Is_A_Non_Null_String()
    {
        Assert.IsTrue(!string.IsNullOrEmpty(Career.GetStrength().Name));
    }
        
    [Test]
    public void Weakness_Is_A_Non_Null_String()
    {
        Assert.IsTrue(!string.IsNullOrEmpty(Career.GetWeakness().Name));
    }
        
    [Test]
    public void Strengths_Are_Not_Null()
    {
        Assert.IsNotNull(Career.GetStrengths());
    }
        
    [Test]
    public void Weaknesses_Are_Not_Null()
    {
        Assert.IsNotNull(Career.GetWeaknesses());
    }
        
    [Test]
    public void TeamValue_Is_Not_Null()
    {
        Assert.IsNotNull(Career.GetTeamValue());
    }
        
    [Test]
    public void WorkEthic_Is_Not_Null()
    {
        Assert.IsNotNull(Career.GetWorkEthic());
    }
}