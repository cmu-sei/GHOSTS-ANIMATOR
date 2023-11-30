// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Text.RegularExpressions;
using Ghosts.Animator.Extensions;
using NUnit.Framework;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class LoremTests
{
	[Test]
	public void Paragraph_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetParagraph(), "[ a-z]+"));
	}
		
	[Test]
	public void Sentence_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetSentence(), "[ a-z]+"));
	}
		
	[Test]
	public void Word_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetWord(), "[ a-z]+"));
	}
		
	[Test]
	public void Paragraphs_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetParagraphs().Join(" "), "[ a-z]+"));
	}
		
	[Test]
	public void Sentences_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetSentences().Join(" "), "[ a-z]+"));
	}
		
	[Test]
	public void Words_Is_A_Non_Null_String()
	{
		Assert.IsTrue(Regex.IsMatch(Lorem.GetWords().Join(" "), "[ a-z]+"));
	}
}