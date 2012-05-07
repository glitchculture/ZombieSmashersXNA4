using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers
{
    class Rand
    {
        public static Random random;

        public static float GetRandomFloat(float fMin, float fMax)
        {
            return (float)random.NextDouble() * (fMax - fMin) + fMin;
        }

        public static double GetRandomDouble(double dMin, double dMax)
        {
            return random.NextDouble() * (dMax - dMin) + dMin;
        }

        public static Vector2 GetRandomVector2(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(GetRandomFloat(xMin, xMax),
                GetRandomFloat(yMin, yMax));
        }

        public static int GetRandomInt(int iMin, int iMax)
        {
            return random.Next(iMax - iMin) + iMin;
        }
    }
}
