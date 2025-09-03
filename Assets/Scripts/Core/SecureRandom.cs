using System;
using System.Security.Cryptography;

namespace Run4theRelic.Core
{
    /// <summary>
    /// Cryptographically-strong RNG utilities for unbiased integer ranges and in-place shuffles.
    /// </summary>
    public static class SecureRandom
    {
        static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Returns a uniformly distributed integer in [0, exclusiveMax).
        /// </summary>
        public static int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));

            // Use rejection sampling to avoid modulo bias
            Span<byte> four = stackalloc byte[4];
            uint limit = (uint.MaxValue / (uint)exclusiveMax) * (uint)exclusiveMax;
            uint value;
            do
            {
                _rng.GetBytes(four);
                value = BitConverter.ToUInt32(four);
            } while (value >= limit);
            return (int)(value % (uint)exclusiveMax);
        }

        /// <summary>
        /// Returns a uniformly distributed integer in [inclusiveMin, exclusiveMax).
        /// </summary>
        public static int NextInt(int inclusiveMin, int exclusiveMax)
        {
            if (exclusiveMax <= inclusiveMin) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            int range = exclusiveMax - inclusiveMin;
            return inclusiveMin + NextInt(range);
        }

        /// <summary>
        /// Fisher–Yates shuffle (in-place) for arrays.
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = NextInt(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// Fisher–Yates shuffle (in-place) for lists.
        /// </summary>
        public static void Shuffle<T>(System.Collections.Generic.IList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = NextInt(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Draws one element index from 0..count-1 excluding a specific forbidden index; returns -1 if no valid choices.
        /// </summary>
        public static int NextIndexExcluding(int count, int forbiddenIndex)
        {
            if (count <= 0) return -1;
            if (count == 1)
            {
                return forbiddenIndex == 0 ? -1 : 0;
            }
            int r = NextInt(count - 1);
            return r >= forbiddenIndex ? r + 1 : r;
        }
    }
}

