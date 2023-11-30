// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class AddressTests
{
	[Test]
	public void City_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetCity(), "[ a-z]+"));
	}

	[Test]
	public void SecondaryAddress_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetSecondaryAddress(), "[ a-z]"));
	}
		
	[Test]
	public void StreetAddress_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetStreetAddress(), "[ a-z]"));
	}
		
	[Test]
	public void StreetName_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetStreetName(), "[ a-z]"));
	}
		
	[Test]
	public void StreetSuffix_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetStreetSuffix(), "[ a-z]"));
	}
		
	[Test]
	public void UKCountry_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetUKCountry(), "[ a-z]"));
	}
		
	[Test]
	public void UKCounty_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetUKCounty(), "[ a-z]"));
	}
		
	[Test]
	public void UKPostcode_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetUKPostcode(), "[ a-z]"));
	}

	[Test]
	public void USStateAbbr_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetUSStateAbbreviation(), "[A-Z]"));
	}
		
	[Test]
	public void USStateName_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetUSStateName(), "[A-Z]"));
	}
		
	[Test]
	public void USZipCode_Is_A_String()
	{
		//Assert.IsTrue(Regex.IsMatch(Address.GetZipCode(), @"[0-9]"));
	}

	[Test]
	public void City_From_Abbrev_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetCityFromStateAbbreviation("AL"), "[ a-zA-Z]"));
	}

	[Test]
	public void GetCityAndZipFromStateAbbreviation_Returns_String_Dictionary()
	{
		var cityAndZip = Address.GetCityAndZipFromStateAbbreviation("AL");
		Assert.IsTrue(Regex.IsMatch(cityAndZip["City"], "[ a-zA-Z]"));
		Assert.IsTrue(Regex.IsMatch(cityAndZip["ZipCode"], "[0-9]"));
	}
		
	[Test]
	public void Neighborhood_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetNeighborhood(), "[ a-z]+"));
	}

	[Test]
	public void WorldCountry_Is_A_String()
	{
		Assert.IsTrue(Regex.IsMatch(Address.GetCountry(), "[ a-z]"));
	}

	[Test]
	public void PopulationData_Returns_Dictionary_of_Strings()
	{
		var addressData = Address.GetFullZipCodeInfo();
		foreach ( var kvp in addressData ) {
			Assert.IsTrue(Regex.IsMatch(kvp.Key, "[ a-zA-Z0-9]"));
			Assert.IsTrue(Regex.IsMatch(kvp.Value, "[ a-zA-Z0-9]"));
		}
	}

}