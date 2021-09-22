/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
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
}