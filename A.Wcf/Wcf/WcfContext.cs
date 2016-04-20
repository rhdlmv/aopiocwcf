namespace AOPIOC.Wcf
{
    using EgoalTech.DB;
    using EgoalTech.UserAccessManage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class WcfContext : IExtension<InstanceContext>
    {
        private readonly IDictionary<string, object> items = new Dictionary<string, object>();

        private WcfContext()
        {
        }

        public void Attach(InstanceContext owner)
        {
        }

        public void Detach(InstanceContext owner)
        {
            Debug.WriteLine(string.Format("Wcf context ({0}) detach.", this.GetHashCode()));
        }

        public List<T> FindAll<T>() where T: class
        {
            List<T> list = new List<T>();
            lock (this)
            {
                foreach (object obj2 in this.items.Values)
                {
                    T item = obj2 as T;
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public T Get<T>(string key)
        {
            object obj2;
            if (this.items.TryGetValue(key, out obj2))
            {
                return (T) obj2;
            }
            return default(T);
        }

        public string GetClientIp()
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty property = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return property.Address;
            }
            return null;
        }

        public int GetClientPort()
        {
            if (OperationContext.Current != null)
            {
                RemoteEndpointMessageProperty property = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return property.Port;
            }
            return 0;
        }

        public DbObjectOperator GetDbObjectOperator()
        {
            return ObjectContainerFactory.GetObjectContainer().Resolve<DbObjectOperator>();
        }

        public DbObjectOperator GetDbObjectOperator(string connectionName)
        {
            return ObjectContainerFactory.GetObjectContainer().Resolve<DbObjectOperator>(connectionName);
        }

        public OperationKey GetOperationKey()
        {
            return this.Get<OperationKey>("OperationKey");
        }

        public UserAccessInfo GetUserAccessInfo()
        {
            return this.Get<UserAccessInfo>("UserAccessInfo");
        }

        public UserAccessManager GetUserAccessManager()
        {
            return ObjectContainerFactory.GetObjectContainer().Resolve<UserAccessManager>();
        }

        public UserAccessManager GetUserAccessManager(string name)
        {
            return ObjectContainerFactory.GetObjectContainer().Resolve<UserAccessManager>(name);
        }

        public void Remove(string key)
        {
            this.items.Remove(key);
        }

        public void Set(string key, object value)
        {
            if (this.items.ContainsKey(key))
            {
                this.items[key] = value;
            }
            else
            {
                this.items.Add(key, value);
            }
        }

        public static WcfContext Current
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    return new WcfContext();
                }
                WcfContext item = OperationContext.Current.InstanceContext.Extensions.Find<WcfContext>();
                if (item == null)
                {
                    item = new WcfContext();
                    OperationContext.Current.InstanceContext.Extensions.Add(item);
                }
                return item;
            }
        }
    }
}

