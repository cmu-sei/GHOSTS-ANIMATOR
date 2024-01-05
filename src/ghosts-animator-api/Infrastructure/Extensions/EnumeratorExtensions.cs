using System;
using System.Collections.Generic;

namespace Ghosts.Animator.Api.Infrastructure.Extensions;

public static class EnumeratorExtensions
{
    public static string GetRandom(this IList<string> list, Random random)
    {
        return list.Count < 1 ? string.Empty : list[random.Next(list.Count)];
    }
}