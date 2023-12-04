// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class CreditCardTests
{
    [Test]
    public void VisaCreditCardNumber_Is_Valid()
    {
        ClassicAssert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.Visa)));
    }

    [Test]
    public void MasterCardCreditCardNumber_Is_Valid()
    {
        ClassicAssert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.MasterCard)));
    }

    [Test]
    public void DinnersClubCreditCardNumber_Is_Valid()
    {
        ClassicAssert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.DinersClub)));
    }
        
    [Test]
    public void GetCard_Is_A_Valid_Card()
    {
        var c = CreditCard.GetCreditCard();
        ClassicAssert.IsNotEmpty(c.Type);
        ClassicAssert.True(IsValidNumber(c.Number));
    }
        
    [Test]
    public void GetCards_Is_An_Array_Of_Valid_Cards()
    {
        var cards = CreditCard.GetCreditCards();
        foreach (var c in cards)
        {
            ClassicAssert.IsNotEmpty(c.Type);
            ClassicAssert.True(IsValidNumber(c.Number));
        }
    }
        
    [Test]
    public void GetCardType_Is_A_Valid_Card_Type()
    {
        ClassicAssert.IsInstanceOf<CreditCard.CardType>(CreditCard.GetCardType());
    }
        
    [Test]
    public void GetNetWorth_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(CreditCard.GetNetWorth());
    }
        
    [Test]
    public void GetTotalDebt_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(CreditCard.GetTotalDebt());
    }
        
    /// <summary>
    /// validate credit card number
    /// </summary>
    private static bool IsValidNumber(string number)
    {
        var deltas = new [] {0, 1, 2, 3, 4, -4, -3, -2, -1, 0};
        var checksum = 0;
        var chars = number.ToCharArray();
        for (var i = chars.Length - 1; i > -1; i--)
        {
            var j = chars[i] - 48;
            checksum += j;
            if ((i - chars.Length) % 2 == 0)
                checksum += deltas[j];
        }

        return checksum % 10 == 0;
    }
}