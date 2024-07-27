using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Rand
{
    private static Random _rand = new Random();

    /// <summary>
    /// Returns a non negative integer less than max.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RandInt(int max)
    {
        return _rand.Next(max);
    }

    /// <summary>
    /// Returns an int between min and max, inclusive.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RandIntRangeInclusive(int min, int max)
    {
        int rangeSize = max - min + 1;
        int randInt = _rand.Next(rangeSize);
        return randInt + min;
    }

    /// <summary>
    /// Returns a random double greater than or equal to 0 and less than 1.
    /// </summary>
    /// <returns></returns>
    public static double RandDouble()
    {
        return _rand.NextDouble();
    }

    /// <summary>
    /// Returns a random float greater than or equal to 0 and less than 1.
    /// </summary>
    /// <returns></returns>
    public static float RandFloat()
    {
        return (float)_rand.NextDouble();
    }

    /// <summary>
    /// Returns a random double within radius around 1. If radius = 0.2, returns in range (0.8, 1.2)
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static double RandDoubleAroundOne(double radius)
    {
        double diameter = radius * 2;
        double change = _rand.NextDouble() * diameter;

        return (1f - radius + change);
    }
}

