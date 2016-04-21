namespace EgoalTech.Commons.Db
{
    using System;
    using System.Text;

    public class SqlStringBuilder
    {
        private StringBuilder sb;

        public SqlStringBuilder()
        {
            this.sb = new StringBuilder();
        }

        public SqlStringBuilder(string str)
        {
            this.sb = new StringBuilder();
            this.Append(str);
        }

        public SqlStringBuilder Append(bool value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(byte value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(char value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(char[] value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(decimal value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(double value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(short value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(int value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(long value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(object value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(float value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(string value)
        {
            this.sb.Append(value);
            return this;
        }

        public SqlStringBuilder Append(char value, int repeatCount)
        {
            this.sb.Append(value, repeatCount);
            return this;
        }

        public SqlStringBuilder Append(char[] value, int startIndex, int charCount)
        {
            this.sb.Append(value, startIndex, charCount);
            return this;
        }

        public SqlStringBuilder Append(string value, int startIndex, int count)
        {
            this.sb.Append(value, startIndex, count);
            return this;
        }

        public SqlStringBuilder AppendFormat(string format, object arg)
        {
            this.sb.AppendFormat(format, this.Escape(arg.ToString()));
            return this;
        }

        public SqlStringBuilder AppendFormat(string format, params object[] args)
        {
            object[] objArray = this.FormatedArgs(args);
            this.sb.AppendFormat(format, objArray);
            return this;
        }

        public SqlStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            object[] objArray = this.FormatedArgs(args);
            this.sb.AppendFormat(provider, format, objArray);
            return this;
        }

        public SqlStringBuilder AppendFormat(string format, object arg0, object arg1)
        {
            this.sb.AppendFormat(format, this.Escape((arg0 == null) ? "" : arg0.ToString()), this.Escape((arg1 == null) ? "" : arg1.ToString()));
            return this;
        }

        public SqlStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            this.sb.AppendFormat(format, this.Escape((arg0 == null) ? "" : arg0.ToString()), this.Escape((arg1 == null) ? "" : arg1.ToString()), this.Escape((arg2 == null) ? "" : arg2.ToString()));
            return this;
        }

        public SqlStringBuilder AppendLine()
        {
            this.sb.AppendLine();
            return this;
        }

        public SqlStringBuilder AppendLine(string value)
        {
            this.sb.AppendLine(value);
            return this;
        }

        public SqlStringBuilder Clear()
        {
            this.sb.Clear();
            return this;
        }

        private string Escape(string str)
        {
            if (str == null)
            {
                return "";
            }
            return str.Replace("'", "''");
        }

        private object[] FormatedArgs(object[] args)
        {
            if (args == null)
            {
                return null;
            }
            object[] objArray = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    objArray[i] = this.Escape(args[i].ToString());
                }
            }
            return objArray;
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }
    }
}

