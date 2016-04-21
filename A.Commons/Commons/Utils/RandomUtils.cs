namespace EgoalTech.Commons.Utils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class RandomUtils
    {
        private static readonly System.Random random = new System.Random();
        private static object syncObj = new object();

        public static object Random(Type type)
        {
            lock (syncObj)
            {
                if (((type == typeof(int)) || (type == typeof(decimal))) || (type == typeof(short)))
                {
                    return random.Next();
                }
                if (type == typeof(double))
                {
                    return random.NextDouble();
                }
            }
            return null;
        }

        public static object Random(Type type, double min, double max)
        {
            lock (syncObj)
            {
                if (((type == typeof(int)) || (type == typeof(decimal))) || (type == typeof(short)))
                {
                    return RandomInt(Convert.ToInt32(min), Convert.ToInt32(max));
                }
                if (type == typeof(double))
                {
                    return RandomDouble(min, max);
                }
            }
            return null;
        }

        public static bool RandomBoolean()
        {
            lock (syncObj)
            {
                return ((random.Next() % 2) == 0);
            }
        }

        public static string RandomDigit(int digits)
        {
            lock (syncObj)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < digits; i++)
                {
                    int num2 = random.Next(0, 10);
                    builder.Append(num2);
                }
                return builder.ToString();
            }
        }

        public static double RandomDouble(bool exceedOne = false)
        {
            lock (syncObj)
            {
                double num = 0.0;
                if (exceedOne)
                {
                    num = random.NextDouble() * random.Next();
                }
                else
                {
                    num = random.NextDouble();
                }
                return num;
            }
        }

        public static double RandomDouble(double min, double max)
        {
            lock (syncObj)
            {
                return ((random.NextDouble() * (max - min)) + max);
            }
        }

        public static int RandomInt()
        {
            lock (syncObj)
            {
                return random.Next();
            }
        }

        public static int RandomInt(int min, int max)
        {
            lock (syncObj)
            {
                return random.Next(min, max);
            }
        }
    }
}

