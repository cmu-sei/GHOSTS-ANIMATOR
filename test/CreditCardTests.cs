/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using NUnit.Framework;

namespace Ghosts.Animator.Tests
{
    [TestFixture]
    public class CreditCardTests
    {
        [Test]
        public void VisaCreditCardNumber_Is_Valid()
        {
            Assert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.Visa)));
        }

        [Test]
        public void MasterCardCreditCardNumber_Is_Valid()
        {
            Assert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.MasterCard)));
        }

        [Test]
        public void DinnersClubCreditCardNumber_Is_Valid()
        {
            Assert.True(IsValidNumber(CreditCard.CreditCardNumber(CreditCard.CardType.DinersClub)));
        }
        
        [Test]
        public void GetCard_Is_A_Valid_Card()
        {
            var c = CreditCard.GetCreditCard();
            Assert.IsNotEmpty(c.Type);
            Assert.True(IsValidNumber(c.Number));
        }
        
        [Test]
        public void GetCards_Is_An_Array_Of_Valid_Cards()
        {
            var cards = CreditCard.GetCreditCards();
            foreach (var c in cards)
            {
                Assert.IsNotEmpty(c.Type);
                Assert.True(IsValidNumber(c.Number));
            }
        }
        
        [Test]
        public void GetCardType_Is_A_Valid_Card_Type()
        {
            Assert.IsInstanceOf<CreditCard.CardType>(CreditCard.GetCardType());
        }
        
        [Test]
        public void GetNetWorth_Is_Not_Null()
        {
            Assert.IsNotNull(CreditCard.GetNetWorth());
        }
        
        [Test]
        public void GetTotalDebt_Is_Not_Null()
        {
            Assert.IsNotNull(CreditCard.GetTotalDebt());
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
                var j = (chars[i]) - 48;
                checksum += j;
                if ((i - chars.Length) % 2 == 0)
                    checksum += deltas[j];
            }

            return ((checksum % 10) == 0);
        }
    }
}