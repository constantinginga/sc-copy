using System;

namespace StartupCentral.Extensions
{
    public static class DoubleExtensions
    {
        public static bool Between(this double value, double oneValue, double otherValue)
        {
            return value >= (oneValue < otherValue ? oneValue : otherValue) && value <= (oneValue > otherValue ? oneValue : otherValue);
        }

        public static double ToPhysologicalPrice(this double value)
        {
            if (value > 1000)
            {
                int zeroPrice = (((Convert.ToInt32(value) / 100) + 1) * 100) - 1;
                return Convert.ToDouble(zeroPrice);
            }
            if (value > 100)
            {
                int zeroPrice = (((Convert.ToInt32(value) / 10) + 1) * 10) - 1;
                return Convert.ToDouble(zeroPrice);
            }
            if (value > 0)
            {
                int zeroPrice = Convert.ToInt32(Math.Ceiling(value));
                return Convert.ToDouble(zeroPrice);
            }

            return value;
        }
    }
}