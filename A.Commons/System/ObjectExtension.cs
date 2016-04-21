namespace System
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ObjectExtension
    {
        private static Dictionary<Type, Func<object, Dictionary<string, string>>> functions = new Dictionary<Type, Func<object, Dictionary<string, string>>>();
        private static Dictionary<string, Action<object, object>> pool = new Dictionary<string, Action<object, object>>();

        public static T CloneTo<T>(this object src) where T: class, new()
        {
            if (src == null)
            {
                return default(T);
            }
            return GetCopyProvider<T>(src.GetType())(src);
        }

        private static Action<object, object> GenCopyProvider(Type fromType, Type toType)
        {
            List<Action<object, object>> list = new List<Action<object, object>>();
            PropertyInfo[] properties = fromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] source = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Func<PropertyInfo, bool> predicate = null;
            foreach (PropertyInfo pFrom in properties)
            {
                if (predicate == null)
                {
                    predicate = p => p.Name == pFrom.Name;
                }
                PropertyInfo info = source.FirstOrDefault<PropertyInfo>(predicate);
                if ((((info != null) && pFrom.CanRead) && pFrom.CanWrite) && (pFrom.PropertyType == info.PropertyType))
                {
                    Func<object, object[], object> getter = new Func<object, object[], object>(pFrom.GetValue);
                    Action<object, object, object[]> setter = new Action<object, object, object[]>(info.SetValue);
                    list.Add(delegate (object from, object to) {
                        object obj2 = getter(from, null);
                        setter(to, obj2, null);
                    });
                }
            }
            return delegate (object from, object to) {
                list.ForEach(f => f(from, to));
            };
        }

        private static Func<object, Dictionary<string, string>> Get(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length == 0)
            {
                throw new Exception(string.Format("The perperties of type({0}) is empty.", type.Name));
            }
            List<Action<object, Dictionary<string, string>>> list = new List<Action<object, Dictionary<string, string>>>();
            foreach (PropertyInfo info in properties)
            {
                string pname = info.Name;
                Func<object, object[], object> getFunction = new Func<object, object[], object>(info.GetValue);
                list.Add(delegate (object o, Dictionary<string, string> dic) {
                    object obj2 = getFunction(o, null);
                    if (obj2 != null)
                    {
                        obj2 = obj2.ToString();
                    }
                    dic.Add(pname, obj2 as string);
                });
            }
            return delegate (object o) {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                list.ForEach(delegate (Action<object, Dictionary<string, string>> additem) {
                    additem(o, dic);
                });
                return dic;
            };
        }

        private static Func<object, T> GetCopyProvider<T>(Type fromType) where T: class, new()
        {
            Action<object, object> fun = null;
            string key = typeof(T).GetHashCode().ToString() + "_" + fromType.GetHashCode().ToString();
            if (!pool.TryGetValue(key, out fun))
            {
                lock (pool)
                {
                    if (!pool.TryGetValue(key, out fun))
                    {
                        fun = GenCopyProvider(fromType, typeof(T));
                        pool.Add(key, fun);
                    }
                }
            }
            return delegate (object o) {
                T local = Activator.CreateInstance<T>();
                fun(o, local);
                return local;
            };
        }

        public static Dictionary<string, string> ToPropertyValues<T>(this T obj)
        {
            return Get(typeof(T))(obj);
        }
    }
}

