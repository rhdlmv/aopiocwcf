using A.Commons.Db;
using A.Commons.Exception;
using A.Commons.Utils;
using A.DB;
using A.DBExtension;
using A.UserAccessManage.Model;
using AOPIOC.Wcf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;

namespace A.WcfCommonOperation
{


    public class CommonOperation
    {
        private string connectionName;
        private QueryParser parser;

        public CommonOperation()
        {
            this.parser = new QueryParser();
            this.connectionName = null;
        }

        public CommonOperation(string connectionName)
        {
            this.parser = new QueryParser();
            this.connectionName = null;
            this.connectionName = connectionName;
        }

        public virtual DbObject Create(User user, string objXml)
        {
            DbObject obj2 = this.Parse(objXml);
            DbObject des = Activator.CreateInstance(obj2.GetType()) as DbObject;
            if (user != null)
            {
                this.SetCreator(obj2, user.Id);
                this.SetLastUpdater(obj2, user.Id);
            }
            A.Commons.Utils.CloneUtils.CloneObject(obj2, des, new string[0]);
            return (this.InvokeGenericMethod(this.DbContext, obj2.GetType(), "Create", new Type[] { obj2.GetType() }, new object[] { obj2 }) as DbObject);
        }

        private string[] GetDefaultModelAssemblyNames()
        {
            string ns = ConfigurationManager.AppSettings["default_model_assembly_name"];
            return SplitStringToArray(ns);
        }

        private string[] GetDefaultModelNamespaces()
        {
            string ns = ConfigurationManager.AppSettings["default_model_namespace"];
            return SplitStringToArray(ns);
        }

        public virtual Type GetObjectType(string objectType, string assemblyName = null)
        {
            string typeName = "";
            bool flag = objectType.Contains<char>('.');
            Type type = null;
            string[] defaultModelNamespaces = this.GetDefaultModelNamespaces();
            if (string.IsNullOrEmpty(assemblyName))
            {
                string[] defaultModelAssemblyNames = this.GetDefaultModelAssemblyNames();
                if (defaultModelAssemblyNames != null)
                {
                    foreach (string str2 in defaultModelAssemblyNames)
                    {
                        if (flag || (defaultModelNamespaces == null))
                        {
                            typeName = objectType + "," + str2;
                        }
                        else
                        {
                            foreach (string str3 in defaultModelNamespaces)
                            {
                                typeName = str3 + "." + objectType + "," + str2;
                                type = Type.GetType(typeName);
                                if (type != null)
                                {
                                    break;
                                }
                            }
                        }
                        if (type != null)
                        {
                            break;
                        }
                        type = Type.GetType(typeName);
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
                else if (flag || (defaultModelNamespaces == null))
                {
                    type = Type.GetType(objectType);
                }
                else
                {
                    foreach (string str3 in defaultModelNamespaces)
                    {
                        type = Type.GetType(str3 + "." + objectType);
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (flag || (defaultModelNamespaces == null))
                {
                    typeName = objectType + "," + assemblyName;
                }
                else
                {
                    foreach (string str3 in defaultModelNamespaces)
                    {
                        typeName = str3 + "." + objectType + "," + assemblyName;
                        type = Type.GetType(typeName);
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
                if (type == null)
                {
                    type = Type.GetType(typeName);
                }
            }
            if (type == null)
            {
                throw new UnknownObjectTypeException("Unknown type: " + objectType);
            }
            return type;
        }

        public virtual Type GetObjectTypeFromXml(string xml)
        {
            string objectType = SerializationUtils.GetObjectType(xml);
            if (StringUtils.IsNullOrEmptyOrWhiteSpace(objectType))
            {
                throw new CommonException("COSE01-006", "Cannot get object type from xml.");
            }
            return this.GetObjectType(objectType, null);
        }

        private object InvokeGenericMethod(object obj, Type genericType, string methodName, Type[] argTypes, object[] args)
        {
            object obj2 = null;
            GenericInvoker invoker = ReflectionUtils.GenericMethodInvokerMethod(obj.GetType(), methodName, new Type[] { genericType }, argTypes);
            try
            {
                obj2 = invoker(obj, args);
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    throw exception.InnerException;
                }
                throw exception;
            }
            return obj2;
        }

        public virtual DbObject Load(string condition, string objectType, bool checkExist = true)
        {
            Type type = this.GetObjectType(objectType, null);
            if (type == null)
            {
                throw new InternalSystemException("Can not find type named '{0}'.");
            }
            string str = this.parser.Parse(condition, type);
            return (this.InvokeGenericMethod(this.DbContext, type, "QueryObject", new Type[] { typeof(string), typeof(bool) }, new object[] { str, checkExist }) as DbObject);
        }

        private DbObject Parse(string xml)
        {
            Type objectTypeFromXml = this.GetObjectTypeFromXml(xml);
            return (DbObject)SerializationUtils.Parse(xml, objectTypeFromXml);
        }

        public virtual IList Query(string condition, string objectType, int rowCount, int pageIndex, string sort)
        {
            Type type = this.GetObjectType(objectType, null);
            string str = this.parser.Parse(condition, type);
            OrderBy[] byArray = this.parser.ParseSort(sort, type);
            IList list = this.InvokeGenericMethod(this.DbContext, type, "QueryObjects", new Type[] { typeof(string), typeof(int), typeof(int), typeof(OrderBy[]) }, new object[] { str, rowCount, pageIndex, byArray }) as IList;
            if (list == null)
            {
                list = new List<DbObject>();
            }
            return list;
        }

        public virtual int QueryCount(string condition, string objectType)
        {
            Type type = this.GetObjectType(objectType, null);
            string str = this.parser.Parse(condition, type);
            object obj2 = this.InvokeGenericMethod(this.DbContext, type, "QueryCount", new Type[] { typeof(string) }, new object[] { str });
            return ((obj2 == null) ? 0 : Convert.ToInt32(obj2));
        }

        public virtual void Remove(string objXml)
        {
            DbObject obj2 = this.Parse(objXml);
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(obj2.GetType()), true);
            string str = obj2.GetValue(pkDynamicPropertyInfo.PropertyName).ToString();
            this.InvokeGenericMethod(this.DbContext, obj2.GetType(), "Delete", new Type[] { typeof(string) }, new object[] { str });
        }

        public virtual void Remove(string id, string objectType)
        {
            Type genericType = this.GetObjectType(objectType, null);
            if (genericType == null)
            {
                throw new InternalSystemException("Can not find type named '{0}'.");
            }
            this.InvokeGenericMethod(this.DbContext, genericType, "Delete", new Type[] { typeof(string) }, new object[] { id });
        }

        private void SetCreator(object obj, string userId)
        {
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(obj.GetType());
            if (DbObjectTools.GetDynamicPropertyInfo(obj.GetType(), "CreateUserId") != null)
            {
                ((DbObject)obj).SetValue<string>("CreateUserId", userId);
            }
        }

        private void SetLastUpdater(object obj, string userId)
        {
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(obj.GetType());
            if (DbObjectTools.GetDynamicPropertyInfo(obj.GetType(), "LastUpdateUserId") != null)
            {
                ((DbObject)obj).SetValue<string>("LastUpdateUserId", userId);
            }
        }

        private static string[] SplitStringToArray(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return null;
            }
            string[] strArray = ns.Split(new char[] { ',' });
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].Trim();
            }
            return strArray;
        }

        public virtual DbObject Update(User user, string objXml)
        {
            DbObject obj2 = this.Parse(objXml);
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(obj2.GetType()), true);
            string arg = obj2.GetValue(pkDynamicPropertyInfo.PropertyName).ToString();
            SqlStringBuilder builder = new SqlStringBuilder(pkDynamicPropertyInfo.DataFieldName);
            builder.AppendFormat(" = '{0}'", arg);
            DbObject des = this.InvokeGenericMethod(this.DbContext, obj2.GetType(), "QueryObject", new Type[] { typeof(string), typeof(bool) }, new object[] { builder.ToString(), true }) as DbObject;
            if (user != null)
            {
                this.SetLastUpdater(obj2, user.Id);
            }
            A.Commons.Utils.CloneUtils.CloneObjectIgnoreId(obj2, des);
            this.InvokeGenericMethod(this.DbContext, obj2.GetType(), "Update", new Type[] { obj2.GetType() }, new object[] { des });
            return des;
        }

        private IDbContext DbContext
        {
            get
            {
                IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
                if (string.IsNullOrEmpty(this.connectionName))
                {
                    return objectContainer.Resolve<IDbContext>();
                }
                return objectContainer.Resolve<IDbContext>(this.connectionName);
            }
        }
    }
}

