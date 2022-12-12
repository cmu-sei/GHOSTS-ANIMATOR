// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Globalization;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
	[TestFixture]
	public class GeoLocationTests
	{
		[Test]
		public void Lat_Is_Valid()
		{
			Assert.IsTrue(Regex.IsMatch(GeoLocation.GetLat().ToString(CultureInfo.InvariantCulture), "[0-9]+"));
		}

		[Test]
		public void Lng_Is_Valid()
		{
			Assert.IsTrue(Regex.IsMatch(GeoLocation.GetLng().ToString(CultureInfo.InvariantCulture), "[0-9]+"));
		}
	}
}