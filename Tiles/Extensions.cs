namespace Tiles
{

    using System;
    using System.Collections.Generic;

    public static class Extensions
    {
        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string ConvertToBase(this long decimalNumber, int radix)
        {
            const int    bitsInLong = 64;
            const string digits     = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > digits.Length)
            {
                throw new ArgumentException("The radix must be >= 2 and <= " + digits.Length);
            }

            if (decimalNumber == 0)
            {
                return "0";
            }

            var    index         = bitsInLong - 1;
            var   currentNumber  = Math.Abs(decimalNumber);
            var   charArray      = new char[bitsInLong];

            while (currentNumber != 0)
            {
                var remainder = (int)(currentNumber % radix);
                charArray[index--] = digits[remainder];
                currentNumber      = currentNumber / radix;
            }

            var result = new string(charArray, index + 1, bitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T>  source,IEqualityComparer<T> comparer = null) => new HashSet<T>(source, comparer);

    }
}
