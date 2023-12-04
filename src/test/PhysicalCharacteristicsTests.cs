// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using Ghosts.Animator.Enums;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class PhysicalCharacteristicsTests
{
    [Test]
    public void Sex_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetBiologicalSex());
    }
    [Test]
    public void Mil_Sex_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilBiologicalSex());
    }

    [Test]
    public void Birthdate_Is_A_Valid_Date()
    {
        var birthday = PhysicalCharacteristics.GetBirthdate();
        ClassicAssert.IsInstanceOf<DateTime>(birthday);
        ClassicAssert.True(DateTime.Now.Year - birthday.Year > 17);
    }
        
    [Test]
    public void Height_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetHeight());
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetHeight(BiologicalSex.Male));
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetHeight(BiologicalSex.Female));
    }

    [Test]
    public void Mil_Height_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilHeight());
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilHeight(MilitaryBranch.USMC));
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilHeight(BiologicalSex.Male));
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilHeight(BiologicalSex.Female));
    }

    [Test]
    public void Mil_Height_Is_Above_Bound()
    {
        for (var i = 0; i < 500; i++)
        {
            ClassicAssert.GreaterOrEqual(80, PhysicalCharacteristics.GetMilHeight());
            ClassicAssert.GreaterOrEqual(78, PhysicalCharacteristics.GetMilHeight(BiologicalSex.Male, MilitaryBranch.USMC));
            ClassicAssert.GreaterOrEqual(72, PhysicalCharacteristics.GetMilHeight(BiologicalSex.Female, MilitaryBranch.USMC));
        }
    }
    [Test]
    public void Mil_Height_Is_Below_Bound()
    {
        for (var i = 0; i < 500; i++)
        {
            ClassicAssert.LessOrEqual(60, PhysicalCharacteristics.GetMilHeight(BiologicalSex.Male));
            ClassicAssert.LessOrEqual(58, PhysicalCharacteristics.GetMilHeight(BiologicalSex.Female));
            ClassicAssert.LessOrEqual(58, PhysicalCharacteristics.GetMilHeight(MilitaryBranch.USMC));
        }
    }

    [Test]
    public void Weight_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetWeight(66));
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetWeight(66, BiologicalSex.Male));
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetWeight(66, BiologicalSex.Female));
    }

    [Test]
    public void Mil_Weight_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(PhysicalCharacteristics.GetMilWeight(70, new DateTime(2000, 4, 1)));
    }
        
    [Test]
    public void Blood_Type_Is_A_Non_Empty_String()
    {
        ClassicAssert.IsNotEmpty(PhysicalCharacteristics.GetBloodType());
    }

    [Test]
    public void Height_To_String_Works()
    {
        ClassicAssert.AreEqual("5' 10\"", PhysicalCharacteristics.HeightToString(70));
    }

    [Test]
    public void Weight_To_String_Works()
    {
        ClassicAssert.AreEqual("100 lbs", PhysicalCharacteristics.WeightToString(100));
    }
}