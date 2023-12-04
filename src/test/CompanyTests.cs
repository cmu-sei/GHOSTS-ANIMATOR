// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class CompanyTests
{
	[Test]
	public void BS_Is_A_Non_Null_String()
	{
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetBS(), "[ a-z]+"));
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetBS(), @"\s"));
	}
		
	[Test]
	public void CatchPhrase_Is_A_Non_Null_String()
	{
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetCatchPhrase(), "[ a-z]+"));
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetCatchPhrase(), @"\s"));
	}
		
	[Test]
	public void CompanyName_Is_A_Non_Null_String()
	{
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetName(), "[ a-z]+"));
	}
		
	[Test]
	public void Suffix_Is_Not_Null()
	{
		ClassicAssert.IsNotNull(Company.GetSuffix());
	}
		
	[Test]
	public void Position_Is_A_Non_Null_String()
	{
		ClassicAssert.IsTrue(Regex.IsMatch(Company.GetPosition(), "[ a-z]+"));
	}
}