namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class DbObjectState
    {
        private Dictionary<string, ValueInfo> initialValues = new Dictionary<string, ValueInfo>();
        private Dictionary<string, ValueInfo> modifiedValues = new Dictionary<string, ValueInfo>();

        private DbObjectInfo GetDbObjectInfo(Type type)
        {
            DbObjectInfo dbObjectInfo = DbObjectReflector.GetDbObjectInfo(type);
            if (dbObjectInfo == null)
            {
                throw new Exception("Cannot retrieve DbObjectInfo of type : " + type.ToString());
            }
            return dbObjectInfo;
        }

        public void PropertyChanged(string name, object value)
        {
            if (this.modifiedValues.ContainsKey(name))
            {
                this.modifiedValues[name].Value = value;
            }
            else
            {
                ValueInfo info = new ValueInfo {
                    Value = value,
                    Name = name
                };
                this.modifiedValues.Add(name, info);
            }
            if (((value != null) && this.initialValues.ContainsKey(name)) && value.Equals(this.initialValues[name]))
            {
                this.modifiedValues.Remove(name);
            }
        }

        public void UpdateInitialValues(DbObject obj)
        {
            this.initialValues.Clear();
            DbObjectInfo dbObjectInfo = this.GetDbObjectInfo(obj.GetType());
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                string key = pair.Key;
                object obj2 = obj.GetValue(key);
                ValueInfo info2 = new ValueInfo {
                    Value = obj2,
                    Name = key
                };
                this.initialValues.Add(key, info2);
            }
        }

        public Dictionary<string, ValueInfo> InitialValues
        {
            get
            {
                return this.initialValues;
            }
        }

        public bool IsModified
        {
            get
            {
                return (this.modifiedValues.Count > 0);
            }
        }

        public bool IsNewObject
        {
            get
            {
                return (!this.ObjectRead && !this.ObjectWrote);
            }
        }

        public Dictionary<string, ValueInfo> ModifiedValues
        {
            get
            {
                return this.modifiedValues;
            }
        }

        public bool ObjectRead { get; set; }

        public bool ObjectWrote { get; set; }
    }
}

