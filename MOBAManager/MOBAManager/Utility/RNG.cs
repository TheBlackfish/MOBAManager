using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Utility
{
    public static class RNG
    {
        /// <summary>
        /// The Random used for generating numbers.
        /// </summary>
        private static Random DICE;

        /// <summary>
        /// Returns a random integer between 0 and max.
        /// </summary>
        /// <param name="max">The maximum number for the roll.</param>
        /// <returns></returns>
        internal static int Roll(int max)
        {
            return DICE.Next(max);
        }

        /// <summary>
        /// Returns a weighted random number.
        /// 0 will always be the most likely number, followed by a curve to the (exclusive) maximum number.
        /// </summary>
        /// <param name="max">The maximum number for the roll.</param>
        /// <returns></returns>
        internal static int RollQuadratic(int max)
        {
            return DICE.Next(max - DICE.Next(max));
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        internal static double RollDouble(double max)
        {
            return DICE.NextDouble() * max;
        }

        /// <summary>
        /// Returns a random true/false value.
        /// </summary>
        /// <returns></returns>
        internal static bool CoinFlip()
        {
            return (DICE.Next(2) == 0);
        }

        /// <summary>
        /// Initializes the RNG.
        /// </summary>
        internal static void InitRNG()
        {
            RNG.DICE = new Random((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}
