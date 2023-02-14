// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class TravelTests
    {
        [Test]
        public void GetTrip_Is_Not_Null()
        {
            Assert.IsNotNull(Travel.GetTrip());
        }
        
        [Test]
        public void GetTrips_Is_Not_Null()
        {
            Assert.IsNotNull(Travel.GetTrips());
        }
    }
}