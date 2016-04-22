namespace A.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class ValidateExtend
    {
        private const string anyLowwerRegex = "[a-z]+";
        private const string asciiRegex = @"^[\x20-\x7E]*$";
        private const string chineseCharacterAsTwoFormat = "(Regard one Chinese word as two characters)";
        private const string chineseOrASCIIRegex = @"^[\u4e00-\u9fa5\x20-\x7E]*$";
        private const string chineseRegex = @"^[\u4e00-\u9fa5]*$";
        private const string decimalRegex = "^[-]?[0]*[0-9]{{0,{0}}}([.][0-9]{{1,{1}}}[0]*)?$";
        private const string emailRegex = @"^[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)*@[a-zA-Z0-9]+[a-zA-Z0-9_-]{0,}\.[a-zA-Z]+(\.[a-zA-Z]+)?$";
        private const string englishAndNumberRegex = "^[a-zA-Z0-9]*$";
        private const string englishLettersRegex = "^[a-zA-Z]*$";
        private const string englishNameRegex = "^[A-Z ]*$";
        private const string equalFormat = "Must be equal to {0}.";
        private const string errorRegexFormat = "Error regex format: {0}.";
        private const string greaterThanFormat = "Must be greater than {0}.";
        private const string greaterThanOrEqualFormat = "Must be greater than or equal to {0}.";
        private const string inFormat = "Must be one of the following: {0}.";
        private const string isACCIIFormat = "Must be a displayable ASCII characters.";
        private const string isChineseFormat = "Must be Chinese characters.";
        private const string isChineseOrASCIIFormat = "Must be Chinese characters or displayable ASCII characters with only Uppercase English characters and common symbols.";
        private const string isDecimalFormat = "Must be a valid numeric format with the percision {0} and scale {1}.";
        private const string isEmailFormat = "Must be a valid email address format.";
        private const string isGuidFormat = "Must be in GUID format.";
        private const string isNumberFormat = "Must be a number";
        private const string isUpperACCIIFormat = "Must be displayable ASCII characters with only Uppercase English characters and common symbols.";
        private const string lengthAboveFormat = "Length of the content must be greater than or equal to {0}.";
        private const string lengthAboveOrEquelFormat = "Length of the content must be greater than or equal to {0} {1}.";
        private const string lengthBelowFormat = "Length of the content must be greater less than {0} {1}.";
        private const string lengthBelowOrEquelFormat = "Length of the content must be less than or equal to {0} {1}.";
        private const string lengthEquelFormat = "Length of the content must be equal to {0} {1}.";
        private const string lessThanFormat = "Must be less than {0}.";
        private const string lessThanOrEqualFormat = "Must be less than or equal to {0}.";
        private const string matchFormat = "Must match the regular expression {0}.";
        private const string notEmptyFormat = "Can not be empty.";
        private const string notEqualFormat = "Can not equal to {0}.";
        private const string notInFormat = "can not be one of the following: {0}.";
        private const string notNullFormat = "Can not be null.";
        private const string nullFormat = "Must contain empty content.";
        private const string numberRegex = "^[0-9]*$";
        private const string upperAlphaEnglishNameRegex = "^[A-Z ]*$";
        private const string usernameRegex = "^[a-zA-Z]{1}[a-zA-Z0-9-_]*$";

        public static T Equal<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = comparisonValue == null;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) == 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must be equal to {0}.", comparisonValue == null ? "null" : comparisonValue.ToString()), field);
            }
            return field;
        }

        public static T GreaterThan<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) > 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must be greater than {0}.", comparisonValue), field);
            }
            return field;
        }

        public static T GreaterThanOrEqual<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) >= 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must be greater than or equal to {0}.", comparisonValue), field);
            }
            return field;
        }

        public static T In<T>(this T field, params T[] comparisonValues) where T : IComparable
        {
            Func<T, object> selector = null;
            bool flag = false;
            foreach (T local in comparisonValues)
            {
                if (field == null)
                {
                    if (local == null)
                    {
                        flag = true;
                    }
                }
                else if (field.CompareTo(local) == 0)
                {
                    flag = true;
                }
                if (flag)
                {
                    break;
                }
            }
            if (flag)
            {
                return field;
            }
            if (comparisonValues.Length == 1)
            {
                throw new ValidateException(string.Format("Must be equal to {0}.", comparisonValues[0]), field);
            }
            StringBuilder builder = new StringBuilder("");
            string str = "";
            for (int i = 0; i < comparisonValues.Count<T>(); i++)
            {
                builder.Append(str);
                builder.Append("{");
                builder.Append(i);
                builder.Append("}");
                str = " / ";
            }
            if (selector == null)
            {
                selector = o => o;
            }
            string str2 = string.Format(builder.ToString(), comparisonValues.Select<T, object>(selector).ToArray<object>());
            throw new ValidateException(string.Format("Must be one of the following: {0}.", str2), field);
        }

        public static string IsASCII(this string field)
        {
            string str;
            try
            {
                str = field.Match<string>(@"^[\x20-\x7E]*$");
            }
            catch (ValidateException)
            {
                throw new ValidateException("Must be a displayable ASCII characters.", field);
            }
            return str;
        }

        public static DateTime IsBirthday(this DateTime field)
        {
            return field.Date.GreaterThanOrEqual<DateTime>(new DateTime(DateTime.Now.Year - 120, 1, 1)).LessThanOrEqual<DateTime>(DateTime.Now.Date);
        }

        public static string IsChinese(this string field)
        {
            string str;
            try
            {
                str = field.Match<string>(@"^[\u4e00-\u9fa5]*$");
            }
            catch (ValidateException)
            {
                throw new ValidateException("Must be Chinese characters.", field);
            }
            return str;
        }

        public static string IsChineseOrASCII(this string field)
        {
            string str;
            try
            {
                str = field.Match<string>(@"^[\u4e00-\u9fa5\x20-\x7E]*$");
            }
            catch (ValidateException)
            {
                throw new ValidateException("Must be Chinese characters or displayable ASCII characters with only Uppercase English characters and common symbols.", field);
            }
            return str;
        }

        public static T IsDecimal<T>(this T field, int numericPercision, int numericScale)
        {
            if (!Regex.IsMatch(field.ToString(), string.Format("^[-]?[0]*[0-9]{{0,{0}}}([.][0-9]{{1,{1}}}[0]*)?$", (numericPercision == -1) ? 0x7fffffff : numericPercision, (numericScale == -1) ? 0x7fffffff : numericScale)))
            {
                int num = (numericPercision == -1) ? 0x7fffffff : numericPercision;
                int num2 = (numericScale == -1) ? 0x7fffffff : numericScale;
                throw new ValidateException(string.Format("Must be a valid numeric format with the percision {0} and scale {1}.", num, num2), field);
            }
            return field;
        }

        public static string IsEmail(this string field)
        {
            string str;
            try
            {
                str = field.Match<string>(@"^[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)*@[a-zA-Z0-9]+[a-zA-Z0-9_-]{0,}\.[a-zA-Z]+(\.[a-zA-Z]+)?$");
            }
            catch (ValidateException)
            {
                throw new ValidateException("Must be a valid email address format.", field);
            }
            return str;
        }

        public static string IsEnglishAndNumber(this string field)
        {
            return field.Match<string>("^[a-zA-Z0-9]*$");
        }

        public static string IsEnglishletters(this string field)
        {
            return field.Match<string>("^[a-zA-Z]*$");
        }

        public static string IsEnglishName(this string field)
        {
            return field.Match<string>("^[A-Z ]*$");
        }

        public static string IsGuid(this string field)
        {
            Guid result = Guid.NewGuid();
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = Guid.TryParse(field, out result) || (field == "");
            }
            if (!flag)
            {
                throw new ValidateException("Must be in GUID format.", field);
            }
            return field;
        }

        public static T IsNumber<T>(this T field)
        {
            if (!Regex.IsMatch(field.ToString(), "^[0-9]*$"))
            {
                throw new ValidateException("Must be a number", field);
            }
            return field;
        }

        public static string IsUpperAlphaEnglishName(this string field)
        {
            return field.Match<string>("^[A-Z ]*$");
        }

        public static string IsUpperASCII(this string field)
        {
            string str;
            try
            {
                field.NotMatch<string>("[a-z]+");
                field.IsASCII();
                str = field;
            }
            catch (ValidateException)
            {
                throw new ValidateException("Must be displayable ASCII characters with only Uppercase English characters and common symbols.", field);
            }
            return str;
        }

        public static string IsUsername(this string field)
        {
            return field.Match<string>("^[a-zA-Z]{1}[a-zA-Z0-9-_]*$");
        }

        public static string LengthAbove(this string field, int length, bool chineseCharacterAsTwo = false)
        {
            if (chineseCharacterAsTwo)
            {
                int num = Convert.ToInt32(Convert.ToChar(0x80));
                foreach (char ch in field)
                {
                    if (Convert.ToInt32(ch) >= num)
                    {
                        length++;
                    }
                }
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.Length > length;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Length of the content must be greater than or equal to {0}.", length, chineseCharacterAsTwo ? "(Regard one Chinese word as two characters)" : ""), field);
            }
            return field;
        }

        public static string LengthAboveOrEqual(this string field, int length, bool chineseCharacterAsTwo = false)
        {
            if (chineseCharacterAsTwo)
            {
                int num = Convert.ToInt32(Convert.ToChar(0x80));
                foreach (char ch in field)
                {
                    if (Convert.ToInt32(ch) >= num)
                    {
                        length++;
                    }
                }
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.Length >= length;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Length of the content must be greater than or equal to {0} {1}.", length, chineseCharacterAsTwo ? "(Regard one Chinese word as two characters)" : ""), field);
            }
            return field;
        }

        public static string LengthBelow(this string field, int length, bool chineseCharacterAsTwo = false)
        {
            if (chineseCharacterAsTwo)
            {
                int num = Convert.ToInt32(Convert.ToChar(0x80));
                foreach (char ch in field)
                {
                    if (Convert.ToInt32(ch) >= num)
                    {
                        length++;
                    }
                }
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.Length < length;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Length of the content must be greater less than {0} {1}.", length, chineseCharacterAsTwo ? "(Regard one Chinese word as two characters)" : ""), field);
            }
            return field;
        }

        public static string LengthBelowOrEqual(this string field, int length, bool chineseCharacterAsTwo = false)
        {
            if (chineseCharacterAsTwo)
            {
                int num = Convert.ToInt32(Convert.ToChar(0x80));
                foreach (char ch in field)
                {
                    if (Convert.ToInt32(ch) >= num)
                    {
                        length++;
                    }
                }
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.Length <= length;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Length of the content must be less than or equal to {0} {1}.", length, chineseCharacterAsTwo ? "(Regard one Chinese word as two characters)" : ""), field);
            }
            return field;
        }

        public static string LengthEqual(this string field, int length, bool chineseCharacterAsTwo = false)
        {
            if (chineseCharacterAsTwo)
            {
                int num = Convert.ToInt32(Convert.ToChar(0x80));
                foreach (char ch in field)
                {
                    if (Convert.ToInt32(ch) >= num)
                    {
                        length++;
                    }
                }
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.Length == length;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Length of the content must be equal to {0} {1}.", length, chineseCharacterAsTwo ? "(Regard one Chinese word as two characters)" : ""), field);
            }
            return field;
        }

        public static T LessThan<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) < 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must be less than {0}.", comparisonValue), field);
            }
            return field;
        }

        public static T LessThanOrEqual<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) <= 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must be less than or equal to {0}.", comparisonValue), field);
            }
            return field;
        }

        public static T Match<T>(this T field, string regex)
        {
            Regex regex2 = null;
            try
            {
                regex2 = new Regex(regex);
            }
            catch
            {
                throw new ValidateException(string.Format("Error regex format: {0}.", regex), field);
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = regex2.IsMatch(field.ToString());
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must match the regular expression {0}.", regex), field);
            }
            return field;
        }

        public static ICollection<T> NotEmpty<T>(this ICollection<T> field)
        {
            if (field.Count <= 0)
            {
                throw new ValidateException("Can not be empty.", field);
            }
            return field;
        }

        public static string NotEmpty(this string field)
        {
            if (!(field != ""))
            {
                throw new ValidateException("Can not be empty.", field);
            }
            return field;
        }

        public static T[] NotEmpty<T>(this T[] field)
        {
            if (field.Length <= 0)
            {
                throw new ValidateException("Can not be empty.", field);
            }
            return field;
        }

        public static T NotEqual<T>(this T field, T comparisonValue) where T : IComparable
        {
            bool flag = false;
            if (field == null)
            {
                flag = comparisonValue != null;
            }
            else
            {
                flag = field.CompareTo(comparisonValue) != 0;
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Can not equal to {0}.", comparisonValue == null ? "null" : comparisonValue.ToString()), field);
            }
            return field;
        }

        public static T NotIn<T>(this T field, params T[] comparisonValues) where T : IComparable
        {
            Func<T, object> selector = null;
            bool flag = true;
            foreach (T local in comparisonValues)
            {
                if ((field == null) && (local == null))
                {
                    flag = false;
                }
                if (field.CompareTo(local) == 0)
                {
                    flag = false;
                }
                if (!flag)
                {
                    break;
                }
            }
            if (flag)
            {
                return field;
            }
            if (comparisonValues.Length == 1)
            {
                throw new ValidateException(string.Format("Can not equal to {0}.", comparisonValues[0]), field);
            }
            StringBuilder builder = new StringBuilder("");
            string str = "";
            for (int i = 0; i < comparisonValues.Count<T>(); i++)
            {
                builder.Append(str);
                builder.Append("{");
                builder.Append(i);
                builder.Append("}");
                str = " / ";
            }
            if (selector == null)
            {
                selector = o => o;
            }
            string str2 = string.Format(builder.ToString(), comparisonValues.Select<T, object>(selector).ToArray<object>());
            throw new ValidateException(string.Format("can not be one of the following: {0}.", str2), field);
        }

        public static T NotMatch<T>(this T field, string regex)
        {
            Regex regex2 = null;
            try
            {
                regex2 = new Regex(regex);
            }
            catch
            {
                throw new ValidateException(string.Format("Error regex format: {0}.", regex), field);
            }
            bool flag = false;
            if (field == null)
            {
                flag = false;
            }
            else
            {
                flag = !regex2.IsMatch(field.ToString());
            }
            if (!flag)
            {
                throw new ValidateException(string.Format("Must match the regular expression {0}.", regex), field);
            }
            return field;
        }

        public static T NotNull<T>(this T field) where T : class
        {
            if (field == null)
            {
                throw new ValidateException("Can not be null.", field);
            }
            return field;
        }

        public static T? NotNull<T>(this T? field) where T : struct
        {
            if (!field.HasValue)
            {
                throw new ValidateException("Can not be null.", field);
            }
            return field;
        }

        public static T Null<T>(this T field) where T : class
        {
            if (field != null)
            {
                throw new ValidateException("Must contain empty content.", field);
            }
            return field;
        }

        public static T? Null<T>(this T? field) where T : struct
        {
            if (field.HasValue)
            {
                throw new ValidateException("Must contain empty content.", field);
            }
            return field;
        }
    }
}

