// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Enums;
using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class MilitaryRankTests
    {
        [Test]
        public void Get_All_Returns_An_Object()
        {
            var o = MilitaryRanks.GetAll();
            Assert.IsTrue(o.Branches.Count == 5);
        }

        [Test]
        public void Get_Returns_A_Rank()
        {
            var o = MilitaryRanks.GetRank();
            Assert.IsTrue(!string.IsNullOrEmpty(o.Name));
        }

        [Test]
        public void Get_By_Branch_Returns_A_Rank()
        {
            var b = MilitaryBranch.USMC;
            var o = MilitaryRanks.GetRankByBranch(b);
            Assert.IsTrue(o.Branch.ToString() == b.ToString());
            
            /* // if we want to test probability spread
            for (int i = 0; i < 100000; i++)
            {
                var o = MilitaryRank.GetRankByBranch("USMC");
                Console.WriteLine($"{o.Pay}\t{o.Name}");
            }
            */
        }
    }
}