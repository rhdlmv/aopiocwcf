namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class DbObjectInfo
    {
        private Dictionary<string, DynamicPropertyInfo> dynamicPropertyInfos = new Dictionary<string, DynamicPropertyInfo>();

        public void AddDynamicPropertyInfo(string propertyName, DynamicPropertyInfo info)
        {
            if (info != null)
            {
                if (this.dynamicPropertyInfos.ContainsKey(propertyName))
                {
                    this.dynamicPropertyInfos[propertyName] = info;
                }
                else
                {
                    this.dynamicPropertyInfos.Add(propertyName, info);
                }
            }
        }

        public string DatabaseName { get; set; }

        public Dictionary<string, DynamicPropertyInfo> DynamicPropertyInfos
        {
            get
            {
                return new Dictionary<string, DynamicPropertyInfo>(this.dynamicPropertyInfos);
            }
        }

        public string QueryTable { get; set; }

        public string Schema { get; set; }

        public string TableName { get; set; }
    }
}

