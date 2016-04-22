namespace A.Commons.Utils
{
    using System;

    public class MathUtils
    {
        public static double Ceiling(double amount, double unit)
        {
            return (Math.Ceiling((double) (amount / unit)) * unit);
        }

        public static double RoundOff(double amount, double unit)
        {
            return (Math.Floor((double) (amount / unit)) * unit);
        }
    }
}

