// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class NameTests
{
	[Test]
	public void Name_Is_Valid()
	{
		Assert.IsTrue(!string.IsNullOrEmpty(Name.GetName().ToString()));
	}
		
	[Test]
	public void FirstName_Is_Valid()
	{
		Assert.IsTrue(!string.IsNullOrEmpty(Name.GetFirstName()));
	}
		
	[Test]
	public void MiddleName_Is_Valid()
	{
		Assert.IsTrue(!string.IsNullOrEmpty(Name.GetMiddleName()));
	}
		
	[Test]
	public void LastName_Is_Valid()
	{
		Assert.IsTrue(!string.IsNullOrEmpty(Name.GetLastName()));
	}
		
	[Test]
	public void Prefix_Is_Valid()
	{
		Assert.IsTrue(Regex.IsMatch(Name.GetPrefix(), @"[A-Z][a-z]+\.?"));
	}
		
	[Test]
	public void Suffix_Is_Valid()
	{
		Assert.IsTrue(Regex.IsMatch(Name.GetSuffix(), @"[A-Z][a-z]*\.?"));
	}
}