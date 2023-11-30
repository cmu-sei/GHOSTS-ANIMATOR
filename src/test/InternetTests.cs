// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class InternetTests
{
	[Test]
	public void Email_Is_A_Valid_Email_Address()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetEmail(), @".+@.+\.\w+"));
	}
		
	[Test]
	public void FreeEmail_Is_A_Valid_Email_Address()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetFreeEmail(), @".+@(gmail|outlook|yahoo)\.com"));
	}
		
	[Test]
	public void DisposableEmail_Is_A_Valid_Email_Address()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetDisposableEmail(), @".+@(mailinator\.com|suremail\.info|spamherelots\.com|binkmail\.com|safetymail\.info)"));
	}
		
	[Test]
	public void UserName_Is_A_Non_Empty_String()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetUserName(), @"[a-z]+((_|\.)[a-z]+)?"));
	}
		
	[Test]
	public void UserNameWithArg_Is_A_Non_Empty_String()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetUserName("bo peep"), @"(bo(_|\.)peep|peep(_|\.)bo)"));
	}
		
	[Test]
	public void DomainName_Is_A_Non_Empty_String()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetDomainName(), @"\w+\.\w+"));
	}
		
	[Test]
	public void DomainWord_Is_A_Non_Empty_String()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetDomainWord(), @"^\w+$"));
	}
		
	[Test]
	public void DomainSuffix_Is_A_Non_Empty_String()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetDomainSuffix(), @"^\w+(\.\w+)?"));
	}
		
	[Test]
	public void Uri_Is_A_Valid_Uri()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetUri("ftp"), @"^ftp:\/\/.+"));
		Assert.IsTrue(Regex.IsMatch(Internet.GetUri("http"), @"^http:\/\/.+"));
		Assert.IsTrue(Regex.IsMatch(Internet.GetUri("https"), @"^https:\/\/.+"));
	}

	[Test] public void TestHttpUrl_Is_A_Valid_Url()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetHttpUrl(), @"^http:\/\/.+"));
	}
		
	[Test]
	public void IP_V4_Address_Is_Valid()
	{
		Assert.IsTrue(Regex.IsMatch(Internet.GetIP_V4_Address(), @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"));
	}
		
	[Test]
	public void Accounts_Returns_A_Non_Empty_Array()
	{
		Assert.IsNotEmpty(Internet.GetAccounts());
	}
		
	[Test]
	public void Account_Is_Not_Null()
	{
		Assert.IsNotNull(Internet.GetAccount());
	}
		
	[Test]
	public void AccountProfile_Is_Not_Null()
	{
		Assert.IsNotNull(Internet.GetAccountProfile());
	}
		
	[Test]
	public void SocialAccount_Is_Not_Null()
	{
		Assert.IsNotNull(Internet.GetSocialAccount());
	}



}