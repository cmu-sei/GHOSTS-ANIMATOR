/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
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
}