/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

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