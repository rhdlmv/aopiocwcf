namespace EgoalTech.Commons.Utils
{
    using EgoalTech.Commons.Exception;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Linq;

    public class SerializationUtils
    {
        private static XElement ArrayToXml(Array array)
        {
            XElement element = new XElement("array");
            foreach (object obj2 in array)
            {
                if (obj2 != null)
                {
                    XElement content = new XElement("item");
                    content.SetAttributeValue("type", GetTypeName(obj2.GetType()));
                    if (obj2.GetType().IsValueType || (obj2.GetType() == typeof(string)))
                    {
                        content.SetValue(obj2);
                    }
                    else
                    {
                        content.Add(ObjectToXml(obj2));
                    }
                    element.Add(content);
                }
            }
            return element;
        }

        private static object ConvertValue(object obj)
        {
            if (obj.GetType() == typeof(DateTime))
            {
                return string.Format("{0:yyyy-MM-ddTHH:mm:sszzz}", obj);
            }
            return obj;
        }

        private static XElement DictionaryToXml(IDictionary dictionary)
        {
            XElement element = new XElement("dictionary");
            foreach (object obj2 in dictionary.Keys)
            {
                XElement content = new XElement("entry");
                content.SetAttributeValue("key", obj2.ToString());
                object obj3 = dictionary[obj2];
                if (obj3 != null)
                {
                    content.SetAttributeValue("type", GetTypeName(obj3.GetType()));
                    if (obj3.GetType().IsValueType || (obj3.GetType() == typeof(string)))
                    {
                        content.SetValue(obj3);
                    }
                    else
                    {
                        content.Add(ObjectToXml(obj3));
                    }
                    element.Add(content);
                }
            }
            return element;
        }

        public static Type GetDataContractType(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(DataContractAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return type;
            }
            Type baseType = type.BaseType;
            if (baseType == null)
            {
                return null;
            }
            return GetDataContractType(baseType);
        }

        public static string GetObjectType(string xml)
        {
            XElement element2 = Load(xml).Element("object");
            if (element2 != null)
            {
                XAttribute attribute = element2.Attribute("type");
                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            return null;
        }

        private static string GetTypeName(Type type)
        {
            string fullName = type.FullName;
            int index = fullName.IndexOf('`');
            if (index != -1)
            {
                fullName = fullName.Substring(0, index);
            }
            return fullName;
        }

        private static bool IsNullable(Type type)
        {
            return (!(type.IsValueType || type.IsClass) || ((Nullable.GetUnderlyingType(type) != null) || type.Equals(typeof(string))));
        }

        private static XElement ListToXml(IList list)
        {
            XElement element = new XElement("list");
            foreach (object obj2 in list)
            {
                if (obj2 != null)
                {
                    XElement content = new XElement("item");
                    content.SetAttributeValue("type", GetTypeName(obj2.GetType()));
                    if (obj2.GetType().IsValueType || (obj2.GetType() == typeof(string)))
                    {
                        content.SetValue(obj2);
                    }
                    else
                    {
                        content.Add(ObjectToXml(obj2));
                    }
                    element.Add(content);
                }
            }
            return element;
        }

        private static XElement Load(string xml)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return XDocument.Load(stream).Element("serialization");
            }
        }

        private static XElement ObjectToXml(object obj)
        {
            if (obj.GetType().IsArray)
            {
                return ArrayToXml(obj as Array);
            }
            if (obj is IList)
            {
                return ListToXml(obj as IList);
            }
            if (obj is IDictionary)
            {
                return DictionaryToXml(obj as IDictionary);
            }
            XElement element = new XElement("object");
            if (obj != null)
            {
                element.SetAttributeValue("type", obj.GetType().Name);
                if (obj.GetType().IsValueType || (obj.GetType() == typeof(string)))
                {
                    element.SetValue(ConvertValue(obj));
                }
                else
                {
                    Type dataContractType = GetDataContractType(obj.GetType());
                    if (dataContractType == null)
                    {
                        throw new EgoalTech.Commons.Exception.SerializationException("Only DataContract can be serialized.");
                    }
                    PropertyInfo[] properties = dataContractType.GetProperties();
                    foreach (PropertyInfo info in properties)
                    {
                        object[] customAttributes = info.GetCustomAttributes(typeof(DataMemberAttribute), true);
                        if ((customAttributes != null) && (customAttributes.Length > 0))
                        {
                            XElement content = new XElement("property");
                            content.SetAttributeValue("name", info.Name);
                            content.SetAttributeValue("type", GetTypeName(info.PropertyType));
                            object obj2 = info.GetValue(obj, null);
                            if (obj2 != null)
                            {
                                if (obj2.GetType().IsValueType || (obj2.GetType() == typeof(string)))
                                {
                                    content.SetAttributeValue("value", ConvertValue(obj2));
                                }
                                else
                                {
                                    content.Add(ObjectToXml(obj2));
                                }
                            }
                            element.Add(content);
                        }
                    }
                }
            }
            return element;
        }

        public static T Parse<T>(string xml)
        {
            return (T) Parse(xml, typeof(T));
        }

        public static object Parse(string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            XElement root = Load(xml);
            object targetObject = null;
            if (type.IsArray)
            {
                targetObject = Activator.CreateInstance(type, new object[] { 0 });
            }
            else
            {
                targetObject = Activator.CreateInstance(type);
            }
            Parse(root, ref targetObject);
            return targetObject;
        }

        private static void Parse(XElement root, ref object targetObject)
        {
            XElement element = root.Element("list");
            if (element != null)
            {
                IList list = XmlToList(element, targetObject.GetType());
                foreach (object obj2 in list)
                {
                    ((IList) targetObject).Add(obj2);
                }
            }
            else
            {
                XElement element2 = root.Element("array");
                if (element2 != null)
                {
                    Array array = XmlToArray(element2, targetObject.GetType());
                    targetObject = array;
                }
                else
                {
                    Type dataContractType = GetDataContractType(targetObject.GetType());
                    if (dataContractType == null)
                    {
                        throw new EgoalTech.Commons.Exception.SerializationException("Only DataContract object can be deserialized.");
                    }
                    object obj3 = Activator.CreateInstance(dataContractType);
                    IEnumerable<XElement> enumerable = root.Element("object").Elements("property");
                    foreach (XElement element4 in enumerable)
                    {
                        ParseProperty(element4, obj3);
                    }
                    CloneUtils.CloneObject(obj3, targetObject, new string[0]);
                }
            }
        }

        private static void ParseProperty(XElement element, object targetObject)
        {
            string name = element.Attribute("name").Value;
            PropertyInfo property = targetObject.GetType().GetProperty(name);
            if (property != null)
            {
                if (property.PropertyType.IsValueType || (property.PropertyType == typeof(string)))
                {
                    XAttribute attribute = element.Attribute("value");
                    if (attribute != null)
                    {
                        string str2 = attribute.Value;
                        Type propertyType = property.PropertyType;
                        if (!(!IsNullable(property.PropertyType) || propertyType.Equals(typeof(string))))
                        {
                            propertyType = Nullable.GetUnderlyingType(propertyType);
                        }
                        object obj2 = Convert.ChangeType(str2, propertyType, null);
                        property.SetValue(targetObject, obj2, null);
                    }
                }
                else if (property.PropertyType.IsArray)
                {
                    XElement element2 = element.Element("array");
                    if (element2 != null)
                    {
                        Array array = XmlToArray(element2, property.PropertyType);
                        property.SetValue(targetObject, array, null);
                    }
                }
                else if (property.PropertyType.GetInterface(typeof(IList).Name, true) != null)
                {
                    XElement element3 = element.Element("list");
                    if (element3 != null)
                    {
                        IList list = XmlToList(element3, property.PropertyType);
                        property.SetValue(targetObject, list, null);
                    }
                }
                else if (property.PropertyType.GetInterface(typeof(IDictionary).Name, true) != null)
                {
                    XElement element4 = element.Element("dictionary");
                    if (element4 != null)
                    {
                        IDictionary dictionary = XmlToDictionary(element4, property.PropertyType);
                        property.SetValue(targetObject, dictionary, null);
                    }
                }
                else
                {
                    XElement element5 = element.Element("object");
                    if (element5 != null)
                    {
                        object obj3 = XmlToObject(element5, property.PropertyType);
                        property.SetValue(targetObject, obj3, null);
                    }
                }
            }
        }

        public static string ToXml(object obj)
        {
            XElement element = new XElement("serialization");
            XElement content = null;
            if (obj.GetType().IsArray)
            {
                content = ArrayToXml(obj as Array);
            }
            else if (obj is IList)
            {
                content = ListToXml(obj as IList);
            }
            else
            {
                content = ObjectToXml(obj);
            }
            element.Add(content);
            return element.ToString();
        }

        private static Array XmlToArray(XElement element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            IEnumerable<XElement> source = element.Elements("item");
            Array array = Array.CreateInstance(type.GetElementType(), source.Count<XElement>());
            int index = 0;
            foreach (XElement element2 in source)
            {
                string typeName = element2.Attribute("type").Value;
                Type conversionType = Type.GetType(typeName);
                if (conversionType == null)
                {
                    string str2 = type.Namespace;
                    Assembly assembly = type.Assembly;
                    if (type.IsGenericType)
                    {
                        conversionType = type.GetGenericArguments()[0];
                    }
                    else
                    {
                        string[] strArray = typeName.Split(new char[] { '.' });
                        conversionType = assembly.GetType(str2 + "." + strArray[strArray.Length - 1]);
                        if (conversionType == null)
                        {
                            continue;
                        }
                    }
                }
                object obj2 = null;
                if (conversionType.IsValueType || (conversionType == typeof(string)))
                {
                    obj2 = Convert.ChangeType(element2.Value, conversionType, null);
                }
                else
                {
                    XElement element3 = element2.Element("object");
                    if (element3 == null)
                    {
                        continue;
                    }
                    obj2 = XmlToObject(element3, conversionType);
                }
                array.SetValue(obj2, index);
                index++;
            }
            return array;
        }

        private static IDictionary XmlToDictionary(XElement element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
            IEnumerable<XElement> enumerable = element.Elements("entry");
            foreach (XElement element2 in enumerable)
            {
                string str = element2.Attribute("key").Value;
                Type conversionType = Type.GetType(element2.Attribute("type").Value);
                if (conversionType != null)
                {
                    object obj2 = null;
                    if (conversionType.IsValueType || (conversionType == typeof(string)))
                    {
                        obj2 = Convert.ChangeType(element2.Value, conversionType, null);
                    }
                    else
                    {
                        obj2 = XmlToObject(element2, conversionType);
                    }
                    dictionary[str] = obj2;
                }
            }
            return dictionary;
        }

        private static IList XmlToList(XElement element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            IList list = Activator.CreateInstance(type) as IList;
            IEnumerable<XElement> enumerable = element.Elements("item");
            foreach (XElement element2 in enumerable)
            {
                string typeName = element2.Attribute("type").Value;
                Type conversionType = Type.GetType(typeName);
                if (conversionType == null)
                {
                    string str2 = type.Namespace;
                    Assembly assembly = type.Assembly;
                    if (type.IsGenericType)
                    {
                        conversionType = type.GetGenericArguments()[0];
                    }
                    else
                    {
                        string[] strArray = typeName.Split(new char[] { '.' });
                        conversionType = assembly.GetType(str2 + "." + strArray[strArray.Length - 1]);
                        if (conversionType == null)
                        {
                            continue;
                        }
                    }
                }
                object obj2 = null;
                if (conversionType.IsValueType || (conversionType == typeof(string)))
                {
                    obj2 = Convert.ChangeType(element2.Value, conversionType, null);
                }
                else
                {
                    XElement element3 = element2.Element("object");
                    if (element3 == null)
                    {
                        continue;
                    }
                    obj2 = XmlToObject(element3, conversionType);
                }
                list.Add(obj2);
            }
            return list;
        }

        private static object XmlToObject(XElement element, Type type)
        {
            Type dataContractType = GetDataContractType(type);
            if (dataContractType == null)
            {
                throw new EgoalTech.Commons.Exception.SerializationException("Only DataContract object can be deserialized.");
            }
            object targetObject = Activator.CreateInstance(dataContractType);
            object des = Activator.CreateInstance(type);
            IEnumerable<XElement> enumerable = element.Elements("property");
            foreach (XElement element2 in enumerable)
            {
                ParseProperty(element2, targetObject);
            }
            CloneUtils.CloneObject(targetObject, des, new string[0]);
            return des;
        }
    }
}

