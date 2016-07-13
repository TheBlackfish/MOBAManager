using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBAManager.Utility
{
    public class RNG
    {
        private static Random dice;

        public static int roll(int max)
        {
            return dice.Next(max);
        }

        public static double rollDouble(double max)
        {
            return dice.NextDouble() * max;
        }

        public static void initRNG()
        {
            RNG.dice = new Random((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}
