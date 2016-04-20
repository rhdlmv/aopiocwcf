namespace AOPIOC.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;

    public class ObjectContainerFactory
    {
        private static Dictionary<string, bool> configCache = new Dictionary<string, bool>();
        private static Dictionary<string, IObjectContainer> containers = new Dictionary<string, IObjectContainer>();
        private static string DEFAULT = "";

        private static void Config(string name, IObjectContainer container)
        {
            if ((container != null) && !configCache.ContainsKey(name))
            {
                IObjectContainerConfig objectContainerConfig = GetObjectContainerConfig(name);
                if (objectContainerConfig != null)
                {
                    objectContainerConfig.Config(container);
                }
                configCache.Add(name, true);
            }
        }

        private static T CreateInstance<T>(string classname)
        {
            T local = default(T);
            Type type = null;
            if (!string.IsNullOrEmpty(classname))
            {
                type = Type.GetType(classname);
            }
            if (type != null)
            {
                local = (T) Activator.CreateInstance(type);
            }
            return local;
        }

        public static IObjectContainer GetObjectContainer()
        {
            return GetObjectContainer(DEFAULT);
        }

        public static IObjectContainer GetObjectContainer(string name)
        {
            IObjectContainer container = null;
            if (!containers.ContainsKey(name))
            {
                string str = name + ("".Equals(name) ? "" : "-") + "object-container";
                if (ConfigurationManager.AppSettings.AllKeys.Contains<string>(str))
                {
                    string classname = ConfigurationManager.AppSettings[str];
                    container = CreateInstance<IObjectContainer>(classname);
                }
                if (container == null)
                {
                    container = new UnityObjectContainer();
                }
                containers.Add(name, container);
            }
            else
            {
                container = containers[name];
            }
            Config(name, container);
            return container;
        }

        private static IObjectContainerConfig GetObjectContainerConfig(string name)
        {
            IObjectContainerConfig config = null;
            string str = name + ("".Equals(name) ? "" : "-") + "object-container-config";
            if (ConfigurationManager.AppSettings.AllKeys.Contains<string>(str))
            {
                string classname = ConfigurationManager.AppSettings[str];
                Debug.WriteLine("Custom wcf config class: " + classname);
                config = CreateInstance<IObjectContainerConfig>(classname);
            }
            if (config == null)
            {
                config = new DefaultUnityConfig();
                Debug.WriteLine("Create default unity config.");
            }
            return config;
        }
    }
}

