namespace EgoalTech.Commons.Utils
{
    using System;
    using System.Collections.Generic;

    public class NumberUtils
    {
        private static int BreakNumberAndAdd(int number)
        {
            char ch;
            string str = number.ToString();
            return ((str.Length == 2) ? (int.Parse(((ch = str[0]) = str[1]).ToString()) + int.Parse(ch.ToString())) : number);
        }

        private static int CalculateLuhnCheckDigit(int total)
        {
            if ((total % 10) == 0)
            {
                return 0;
            }
            return (10 - (total % 10));
        }

        public static int GetLuhnCheckDigit(string str)
        {
            int total = 0;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if ((i % 2) == 0)
                {
                    char ch = str[i];
                    total += Convert.ToInt32(ch.ToString());
                }
                else
                {
                    total += BreakNumberAndAdd(MultiplyByTwo(str[i]));
                }
            }
            return CalculateLuhnCheckDigit(total);
        }

        private static int MultiplyByTwo(char digit)
        {
            return (int.Parse(digit.ToString()) * 2);
        }

        public static int[] Split(int number)
        {
            if (number <= 0)
            {
                return null;
            }
            List<int> list = new List<int>();
            while (number != 0)
            {
                list.Add(number % 10);
                number /= 10;
            }
            return list.ToArray();
        }

        public static int[] SplitWithFixLength(int number, int lengh)
        {
            if (lengh <= 0)
            {
                return null;
            }
            int[] numArray = Split(number);
            int length = numArray.Length;
            decimal d = length / lengh;
            int[] numArray2 = new int[Convert.ToInt32(Math.Ceiling(d))];
            int num4 = 1;
            int num5 = 0;
            int index = -1;
            for (int i = 0; i < length; i++)
            {
                if ((i % lengh) == 0)
                {
                    if (i != 0)
                    {
                        numArray2[index] = num5;
                        num5 = 0;
                    }
                    num4 = 1;
                    index++;
                }
                num5 += numArray[i] * num4;
                num4 *= 10;
            }
            numArray2[index] = num5;
            return numArray2;
        }
    }
}

