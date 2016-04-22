namespace AOPIOC.Wcf
{
    using System;
    using System.Collections.Generic;
    

    public class OperationKey
    {
        public T GetProperty<T>(string key)
        {
            if (this.Properties.ContainsKey(key))
            {
                return (T) this.Properties[key];
            }
            return default(T);
        }

        public Dictionary<string, object> Properties { get; set; }

        public string SessionId { get; set; }
    }
}

