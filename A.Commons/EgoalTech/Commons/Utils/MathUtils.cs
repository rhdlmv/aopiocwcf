namespace EgoalTech.Commons.Utils
{
    using System;

    public class MathUtils
    {
        public static double Ceiling(double amount, double unit) => 
            (Math.Ceiling((double) (amount / unit)) * unit)

        public static double RoundOff(double amount, double unit) => 
            (Math.Floor((double) (amount / unit)) * unit)
    }
}

