namespace EgoalTech.Commons.Exception
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Linq;

    public abstract class GenericException : Exception
    {
        protected string _message;
        private Dictionary<string, string> extraDatas;

        public GenericException()
        {
            this.extraDatas = new Dictionary<string, string>();
        }

        public GenericException(Exception ex) : base("", ex)
        {
            this.extraDatas = new Dictionary<string, string>();
            this._message = ex.Message;
        }

        public GenericException(string message)
        {
            this.extraDatas = new Dictionary<string, string>();
            this._message = message;
        }

        public GenericException(string message, Exception ex) : base("", ex)
        {
            this.extraDatas = new Dictionary<string, string>();
            this._message = message;
        }

        public void AddExtraData(string key, string value)
        {
            this.extraDatas[key] = value;
        }

        public void ClearExtraData()
        {
            this.extraDatas.Clear();
        }

        private string GetValidString(string str)
        {
            if (str == null)
            {
                return "";
            }
            return str;
        }

        public void RemoveExtraData(string key)
        {
            this.extraDatas.Remove(key);
        }

        private string ToXml()
        {
            XElement element = new XElement("exception");
            XElement content = new XElement("code") {
                Value = this.GetValidString(this.ErrorCode)
            };
            element.Add(content);
            content = new XElement("name") {
                Value = this.GetValidString(base.GetType().FullName)
            };
            element.Add(content);
            content = new XElement("system_error") {
                Value = this.GetValidString(this.SystemError.ToString())
            };
            element.Add(content);
            content = new XElement("message") {
                Value = this.GetValidString(this._message)
            };
            element.Add(content);
            content = new XElement("stacktrace");
            StringBuilder builder = new StringBuilder();
            builder.Append(this.StackTrace);
            for (Exception exception = base.InnerException; exception != null; exception = exception.InnerException)
            {
                builder.Append("\n\r");
                builder.Append(exception.StackTrace);
            }
            content.Value = this.GetValidString(builder.ToString());
            element.Add(content);
            if (this.extraDatas.Count != 0)
            {
                content = new XElement("extra-data");
                foreach (KeyValuePair<string, string> pair in this.extraDatas)
                {
                    XElement element3 = new XElement(pair.Key);
                    element3.SetValue(pair.Value);
                    content.Add(element3);
                }
                element.Add(content);
            }
            return element.ToString();
        }

        public abstract string ErrorCode { get; }

        public override string Message
        {
            get
            {
                return this.ToXml();
            }
        }

        public bool SystemError { get; set; }
    }
}

