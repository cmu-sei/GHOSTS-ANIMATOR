// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Ghosts.Animator.Tests;

[TestFixture]
public class MentalHealthTests
{
    [Test]
    public void IQ_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(MentalHealth.GetIQ());
    }
        
    [Test]
    public void MentalHealth_Is_Not_Null()
    {
        ClassicAssert.IsNotNull(MentalHealth.GetMentalHealth());
    }
}