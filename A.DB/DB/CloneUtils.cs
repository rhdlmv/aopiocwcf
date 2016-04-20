namespace EgoalTech.DB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class CloneUtils
    {
        public static void CloneDictionary(IDictionary src, IDictionary des)
        {
            IDictionaryEnumerator enumerator = src.GetEnumerator();
            while (enumerator.MoveNext())
            {
                des.Add(enumerator.Key, enumerator.Value);
            }
        }

        public static void CloneList(IList src, IList des)
        {
            for (int i = 0; i < src.Count; i++)
            {
                des.Add(src[i]);
            }
        }

        public static void CloneObject<T>(T src, T des, params Expression<Func<T, object>>[] ignoreFields)
        {
            List<string> list = new List<string>();
            if (ignoreFields != null)
            {
                foreach (Expression<Func<T, object>> expression in ignoreFields)
                {
                    UnaryExpression body = expression.Body as UnaryExpression;
                    if (body != null)
                    {
                        string name = (body.Operand as MemberExpression).Member.Name;
                        list.Add(name);
                    }
                }
            }
            CloneObject(src, des, list.ToArray());
        }

        public static void CloneObject(object src, object des, params string[] ignoreFields)
        {
            PropertyInfo[] properties = des.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (!ignoreFields.Contains<string>(properties[i].Name))
                {
                    PropertyInfo property = src.GetType().GetProperty(properties[i].Name);
                    if ((property != null) && property.PropertyType.Equals(properties[i].PropertyType))
                    {
                        object obj3;
                        object obj2 = property.GetValue(src, null);
                        if (properties[i].PropertyType.IsSubclassOf(typeof(IList)))
                        {
                            obj3 = properties[i].GetValue(src, null);
                            if ((obj3 == null) && properties[i].CanWrite)
                            {
                                obj3 = Activator.CreateInstance(properties[i].PropertyType);
                                properties[i].SetValue(des, obj3, null);
                            }
                            CloneList(obj2 as IList, obj3 as IList);
                        }
                        else if (properties[i].PropertyType.IsSubclassOf(typeof(IDictionary)))
                        {
                            obj3 = properties[i].GetValue(src, null);
                            if ((obj3 == null) && properties[i].CanWrite)
                            {
                                obj3 = Activator.CreateInstance(properties[i].PropertyType);
                                properties[i].SetValue(des, obj3, null);
                            }
                            CloneDictionary(obj2 as IDictionary, obj3 as IDictionary);
                        }
                        else if (properties[i].CanWrite)
                        {
                            properties[i].SetValue(des, obj2, null);
                        }
                    }
                }
            }
        }

        public static void CloneObjectIgnoreId(object src, object des)
        {
            CloneObject(src, des, new string[] { "Id" });
        }
    }
}

