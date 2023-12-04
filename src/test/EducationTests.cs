// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Models;
using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class EducationTests
{
    [Test]
    public void Education_Profile_To_String_Works()
    {
        var e = new EducationProfile();
        var tmp = new School
        {
            Name = "TestSchool",
            Location = "USA"
        };
        e.Degrees = new List<EducationProfile.Degree>
        {
            new(Enums.DegreeLevel.Bachelors, "Computer Science,B.S.", tmp),
            new(Enums.DegreeLevel.Masters, "Computer Science,M.S.", tmp)
        };
        ClassicAssert.AreEqual(e.ToString(), "B.S. in Computer Science from TestSchool\nM.S. in Computer Science from TestSchool\n");
    }
        
    [Test]
    public void Degree_Level_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetDegreeLevel());
    }

    [Test]
    public void Major_Is_A_Non_Null_String()
    {
        for (var i = 0; i < 50; i++)
        {
            ClassicAssert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Associates));
            ClassicAssert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Bachelors));
            ClassicAssert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Masters));
            ClassicAssert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Doctorate));
            ClassicAssert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Professional));
        }
    }

    [Test]
    public void Major_Is_A_Null_String()
    {
        for(var i=0; i<50; i++)
        {
            ClassicAssert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.None));
            ClassicAssert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.GED));
            ClassicAssert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.HSDiploma));

        }
    }
        
    [Test]
    public void School_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetSchool());
        ClassicAssert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Associates));
        ClassicAssert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Professional, "M.D."));
        ClassicAssert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Professional, "J.D."));
    }

    [Test]
    public void US_School_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetUSSchool());
        ClassicAssert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Associates));
        ClassicAssert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Professional, "M.D."));
        ClassicAssert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Professional, "J.D."));
    }

    [Test]
    public void US_School_Works()
    {
        ClassicAssert.AreEqual("USA", Education.GetUSSchool().Location);
        ClassicAssert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Associates).Location);
        ClassicAssert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Professional, "M.D.").Location);
        ClassicAssert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Professional, "J.D.").Location);
    }

    [Test]
    public void Get_Education_Profile_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetEducationProfile());
    }

    [Test]
    public void Get_Education_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetEducation());
    }

    [Test]
    public void Get_Mil_Education_Profile_Is_Not_Null()
    {
        for (var i = 0; i < 100; i++)
        {
            var rank = MilitaryRanks.GetRank();
            /*
            rank.Pay = "O-6";
            rank.MOSID = "4400";
            rank.Branch = Enums.MilitaryBranch.USAF;
            */
            ClassicAssert.IsNotNull(Education.GetMilEducationProfile(rank));
        }
    }

    [Test]
    public void Get_Mil_Education_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-1"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-2"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-3"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-4"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-5"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-6"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-7"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-8"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-9"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("E-10"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("W-1"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("W-2"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("W-3"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("W-4"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("W-5"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-1"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-2"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-3"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-4"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-5"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-6"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-7"));
        ClassicAssert.IsNotNull(Education.GetMilEducation("O-8"));
    }
}