/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ghosts.Animator.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var elements = source.ToArray();
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                var swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }
        
        public static string RandomFromProbabilityList(this Dictionary<string, double> list)
        {
            var u = list.Sum(x => x.Value);
            var r = AnimatorRandom.Rand.NextDouble() * u;
            double sum = 0;
            return list.FirstOrDefault(x => r <= (sum += x.Value)).Key;
        }

        public static int RandomFromPipedProbabilityList(this Dictionary<string, double> list)
        {
            var selected = list.RandomFromProbabilityList();
            var arr = selected.Split(Convert.ToChar("|"));
            return AnimatorRandom.Rand.Next(Convert.ToInt32(arr[0]), Convert.ToInt32(arr[1]));
        }
        
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing(new Random());
        }
        
        public static string Join<T>(this IEnumerable<T> items, string separator)
        {
            return items.Select(i => i.ToString())
                .Aggregate((acc, next) => string.Concat(acc, separator, next));
        }

        public static IEnumerable<T> RandPick<T>(this IEnumerable<T> items, int itemsToTake)
        {
            IList<T> list;
            if (items is IList<T> list1)
                list = list1;
            else list = items.ToList();

            var rand = AnimatorRandom.Rand;

            for (var i = 0; i < itemsToTake; i++)
                yield return list[rand.Next(list.Count)];
        }
        
        public static string RandomFromStringArray(this string[] ar)
        {
            var rand = AnimatorRandom.Rand;
            return ar[rand.Next(ar.Length)];
        }
        
        private static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            var e = enumerable.ToList();
            if(e.Count==0)
            {
                return default(T);
            }
            return e.ElementAt(rand.Next(0, e.Count));
        }
    }
}