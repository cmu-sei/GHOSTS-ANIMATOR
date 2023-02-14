// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class EducationTests
    {
        [Test]
        public void Education_Profile_To_String_Works()
        {
            var e = new EducationProfile();
            School tmp = new School();
            tmp.Name = "TestSchool";
            tmp.Location = "USA";
            e.Degrees = new List<EducationProfile.Degree>();
            e.Degrees.Add(new EducationProfile.Degree(Enums.DegreeLevel.Bachelors, "Computer Science,B.S.", tmp));
            e.Degrees.Add(new EducationProfile.Degree(Enums.DegreeLevel.Masters, "Computer Science,M.S.", tmp));
            Assert.AreEqual(e.ToString(), "B.S. in Computer Science from TestSchool\nM.S. in Computer Science from TestSchool\n");
        }
        
        [Test]
        public void Degree_Level_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetDegreeLevel());
        }

        [Test]
        public void Major_Is_A_Non_Null_String()
        {
            for (int i = 0; i < 50; i++)
            {
                Assert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Associates));
                Assert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Bachelors));
                Assert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Masters));
                Assert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Doctorate));
                Assert.IsNotEmpty(Education.GetMajor(Enums.DegreeLevel.Professional));
            }
        }

        [Test]
        public void Major_Is_A_Null_String()
        {
            for(int i=0; i<50; i++)
            {
                Assert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.None));
                Assert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.GED));
                Assert.IsEmpty(Education.GetMajor(Enums.DegreeLevel.HSDiploma));

            }
        }
        
        [Test]
        public void School_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetSchool());
            Assert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Associates));
            Assert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Professional, "M.D."));
            Assert.IsNotNull(Education.GetSchool(Enums.DegreeLevel.Professional, "J.D."));
        }

        [Test]
        public void US_School_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetUSSchool());
            Assert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Associates));
            Assert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Professional, "M.D."));
            Assert.IsNotNull(Education.GetUSSchool(Enums.DegreeLevel.Professional, "J.D."));
        }

        [Test]
        public void US_School_Works()
        {
            Assert.AreEqual("USA", Education.GetUSSchool().Location);
            Assert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Associates).Location);
            Assert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Professional, "M.D.").Location);
            Assert.AreEqual("USA", Education.GetUSSchool(Enums.DegreeLevel.Professional, "J.D.").Location);
        }

        [Test]
        public void Get_Education_Profile_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetEducationProfile());
        }

        [Test]
        public void Get_Education_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetEducation());
        }

        [Test]
        public void Get_Mil_Education_Profile_Is_Not_Null()
        {
            for (int i = 0; i < 100; i++)
            {
                var rank = MilitaryRanks.GetRank();
                /*
                rank.Pay = "O-6";
                rank.MOSID = "4400";
                rank.Branch = Enums.MilitaryBranch.USAF;
                */
                Assert.IsNotNull(Education.GetMilEducationProfile(rank));
            }
        }

        [Test]
        public void Get_Mil_Education_Is_Not_Null()
        {
            Assert.IsNotNull(Education.GetMilEducation("E-1"));
            Assert.IsNotNull(Education.GetMilEducation("E-2"));
            Assert.IsNotNull(Education.GetMilEducation("E-3"));
            Assert.IsNotNull(Education.GetMilEducation("E-4"));
            Assert.IsNotNull(Education.GetMilEducation("E-5"));
            Assert.IsNotNull(Education.GetMilEducation("E-6"));
            Assert.IsNotNull(Education.GetMilEducation("E-7"));
            Assert.IsNotNull(Education.GetMilEducation("E-8"));
            Assert.IsNotNull(Education.GetMilEducation("E-9"));
            Assert.IsNotNull(Education.GetMilEducation("E-10"));
            Assert.IsNotNull(Education.GetMilEducation("W-1"));
            Assert.IsNotNull(Education.GetMilEducation("W-2"));
            Assert.IsNotNull(Education.GetMilEducation("W-3"));
            Assert.IsNotNull(Education.GetMilEducation("W-4"));
            Assert.IsNotNull(Education.GetMilEducation("W-5"));
            Assert.IsNotNull(Education.GetMilEducation("O-1"));
            Assert.IsNotNull(Education.GetMilEducation("O-2"));
            Assert.IsNotNull(Education.GetMilEducation("O-3"));
            Assert.IsNotNull(Education.GetMilEducation("O-4"));
            Assert.IsNotNull(Education.GetMilEducation("O-5"));
            Assert.IsNotNull(Education.GetMilEducation("O-6"));
            Assert.IsNotNull(Education.GetMilEducation("O-7"));
            Assert.IsNotNull(Education.GetMilEducation("O-8"));
        }
    }
}