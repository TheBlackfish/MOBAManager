using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Utility
{
    public class RNG
    {
        /// <summary>
        /// The Random used for generating numbers.
        /// </summary>
        private static Random dice;

        /// <summary>
        /// Returns a random integer between 0 and max.
        /// </summary>
        /// <param name="max">The maximum number for the roll.</param>
        /// <returns></returns>
        public static int roll(int max)
        {
            return dice.Next(max);
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double rollDouble(double max)
        {
            return dice.NextDouble() * max;
        }

        /// <summary>
        /// Initializes the RNG.
        /// </summary>
        public static void initRNG()
        {
            RNG.dice = new Random((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}
