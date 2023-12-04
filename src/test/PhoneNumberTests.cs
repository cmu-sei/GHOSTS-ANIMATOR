// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class PhoneNumberTests
{
	[Test]
	public void PhoneNumber_Is_Valid()
	{
		ClassicAssert.IsTrue(Regex.IsMatch(PhoneNumber.GetPhoneNumber(), @"\d{3}[. -]\d{3}"));
	}
}