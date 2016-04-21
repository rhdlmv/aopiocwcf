namespace A.DBExtension
{
    using EgoalTech.Commons.Db;
    using EgoalTech.Commons.Exception;
    using EgoalTech.DB;
    using A.Validation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class RuleEngine<T> : ObjectValidator<T>, IDbObjectInfo<T>, IModelValidator<T>, IRuleMapper<T> where T: DbObject, new()
    {
        public RuleEngine()
        {
            this.Init();
            this.SetDefault();
            this.Bind();
        }

        public IRuleMapper<T> AddCompositeUniqueKeyFields(params Expression<Func<T, object>>[] fields)
        {
            List<DataFieldAttribute> source = new List<DataFieldAttribute>();
            foreach (Expression<Func<T, object>> expression in fields)
            {
                string fieldname = this.GetPropertyName(expression);
                if (source.All<DataFieldAttribute>(t => t.ObjectFieldName != fieldname))
                {
                    DataFieldAttribute item = this.GetDbField(fieldname, true, true);
                    source.Add(item);
                }
            }
            this.CompositeUniqueKeyFields.Add(source.ToArray());
            return this;
        }

        public IRuleMapper<T> AddCreateExceptFields(params Expression<Func<T, object>>[] fields)
        {
            List<DataFieldAttribute> createExceptFields = this.CreateExceptFields;
            foreach (Expression<Func<T, object>> expression in fields)
            {
                string fieldname = this.GetPropertyName(expression);
                if (createExceptFields.All<DataFieldAttribute>(t => t.ObjectFieldName != fieldname))
                {
                    DataFieldAttribute item = this.GetDbField(fieldname, true, true);
                    createExceptFields.Add(item);
                }
            }
            return this;
        }

        public IRuleMapper<T> AddUpdateExceptFields(params Expression<Func<T, object>>[] fields)
        {
            List<DataFieldAttribute> updateExceptFields = this.UpdateExceptFields;
            foreach (Expression<Func<T, object>> expression in fields)
            {
                string fieldname = this.GetPropertyName(expression);
                if (updateExceptFields.All<DataFieldAttribute>(t => t.ObjectFieldName != fieldname))
                {
                    DataFieldAttribute item = this.GetDbField(fieldname, true, true);
                    updateExceptFields.Add(item);
                }
            }
            return this;
        }

        public void Bind()
        {
            if (this.DbTable != null)
            {
                if (this.PrimaryKeyField == null)
                {
                    Exception exception = new Exception("The PrimaryKey of model({0}) is not set.");
                    throw exception;
                }
                this.BindCreateCloneFunction();
                this.BindUpdateCloneFunction();
                this.BindCheckReduplicationFunction();
                this.BindRemoveCloneFunction();
                this.BindGetPrimaryKeyFunction();
                this.BindGetModifyDatetimeFunction();
                this.BindSetModifyDatetimeFunction();
                this.BindSetLogicalDeleteFunction();
            }
        }

        private void BindCheckReduplicationFunction()
        {
            this.CheckDuplicateFunction = null;
            if (this.CompositeUniqueKeyFields.Count > 0)
            {
                List<Action<IDbContext, T, string[]>> list = new List<Action<IDbContext, T, string[]>>();
                using (List<DataFieldAttribute[]>.Enumerator enumerator = this.CompositeUniqueKeyFields.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        DataFieldAttribute[] arr = enumerator.Current;
                        Func<T, string> getRepeatConditionFunction;
                        string modelName;
                        if ((arr != null) && (arr.Length != 0))
                        {
                            //<>c__DisplayClass61<T> class3;
                            //<>c__DisplayClass5f<T> classf;
                            string[] strArray = (from t in arr select t.ObjectFieldName).ToArray<string>();
                            getRepeatConditionFunction = this.GetRepeatConditionFunction(arr);
                            modelName = typeof(T).Name;
                            Action<IDbContext, T, string[]> item = delegate (IDbContext context, T model, string[] fields) {
                                //<>c__DisplayClass61<T> class1 = class3;
                                //<>c__DisplayClass5f<T> classf2 = classf;
                                if (((fields == null) || (fields.Length <= 0)) || arr.Any<DataFieldAttribute>(delegate (DataFieldAttribute i) {
                                    //<>c__DisplayClass61<T> class2 = class1;
                                    //<>c__DisplayClass5f<T> classf1 = classf2;
                                    return fields.Any<string>(f => (f == i.DataFieldName));
                                }))
                                {
                                    string condition = "1=1" + getRepeatConditionFunction(model);
                                    if (context.QueryObject<T>(condition, false) != null)
                                    {
                                        string str2 = "";
                                        string str3 = "";
                                        DataFieldAttribute[] attributeArray1 = arr;
                                        for (int k = 0; k < attributeArray1.Length; k++)
                                        {
                                            Func<PropertyInfo, bool> predicate = null;
                                            //<>c__DisplayClass61<T> class3 = class3;
                                            //<>c__DisplayClass5f<T> classf3 = classf;
                                            DataFieldAttribute field = attributeArray1[k];
                                            if (predicate == null)
                                            {
                                                predicate = t => t.Name == field.ObjectFieldName;
                                            }
                                            PropertyInfo info = this.PropertyInfos.First<PropertyInfo>(predicate);
                                            object obj2 = info.GetValue(model, null);
                                            if (info.PropertyType != typeof(string))
                                            {
                                                obj2 = obj2.FormatToSql();
                                            }
                                            str2 = str2 + string.Format("{2}{0}={1}", info.Name, obj2, str3);
                                            str3 = ",";
                                        }
                                        throw new ObjectAlreadyExistException(typeof(T), string.Format("The {0}({1}) is reduplicate.", modelName, str2));
                                    }
                                }
                            };
                            list.Add(item);
                        }
                    }
                }
                this.CheckDuplicateFunction = (context, model, fields) => list.ForEach(checkFunction => checkFunction(context, model, fields));
            }
        }

        private void BindCreateCloneFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            this.CreateCloneFunction = null;
            if (this.CreateExceptFields.Count > 0)
            {
                if (predicate == null)
                {
                    predicate = t => t.Name == base.PrimaryKeyField.ObjectFieldName;
                }
                PropertyInfo idProperty = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                Func<T, T, T> cloneFunction = this.GetCloneModelFunction((from t in this.CreateExceptFields select t.ObjectFieldName).ToArray<string>());
                this.CreateCloneFunction = delegate (IDbContext context, T model) {
                    T local = Activator.CreateInstance<T>();
                    object obj2 = idProperty.GetValue(model, null) ?? Guid.NewGuid().ToString();
                    idProperty.SetValue(local, obj2, null);
                    idProperty.SetValue(model, obj2, null);
                    cloneFunction(model, local);
                    return local;
                };
            }
        }

        private void BindGetModifyDatetimeFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            this.GetModifyDatetimeFunction = null;
            if (this.UpdateDatetimeField != null)
            {
                if (predicate == null)
                {
                    predicate = t => t.Name == base.UpdateDatetimeField.ObjectFieldName;
                }
                PropertyInfo property = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                this.GetModifyDatetimeFunction = model => (DateTime) property.GetValue(model, null);
            }
        }

        private void BindGetPrimaryKeyFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            this.GetPrimaryKeyFunction = null;
            if (this.PrimaryKeyField != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.Name == base.PrimaryKeyField.ObjectFieldName;
                }
                PropertyInfo info = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                Func<object, object[], object> getter = new Func<object, object[], object>(info.GetValue);
                this.GetPrimaryKeyFunction = m => getter(m, null) as string;
            }
        }

        private void BindRemoveCloneFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            Func<DataFieldAttribute, bool> func2 = null;
            this.RemoveCloneFunction = null;
            if (this.LogicalDeleteField != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.Name == base.LogicalDeleteField.ObjectFieldName;
                }
                PropertyInfo property = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                if (func2 == null)
                {
                    func2 = f => f.ObjectFieldName != base.LogicalDeleteField.ObjectFieldName;
                }
                DataFieldAttribute[] attributeArray = this.AllFields.Where<DataFieldAttribute>(func2).ToArray<DataFieldAttribute>();
                Func<T, T, T> cloneFunction = this.GetCloneModelFunction((from t in attributeArray select t.ObjectFieldName).ToArray<string>());
                this.RemoveCloneFunction = delegate (IDbContext context, T model) {
                    DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(typeof(T)), true);
                    object obj2 = model.GetValue(pkDynamicPropertyInfo.PropertyName);
                    T local = context.QueryObject<T>(pkDynamicPropertyInfo.DataFieldName, obj2, false);
                    if (local != null)
                    {
                        cloneFunction(model, local);
                        property.SetValue(local, true, null);
                        property.SetValue(model, true, null);
                    }
                    return local;
                };
            }
        }

        private void BindSetLogicalDeleteFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            this.SetLogicalDeleteFunction = null;
            if (this.LogicalDeleteField != null)
            {
                if (predicate == null)
                {
                    predicate = t => t.Name == base.LogicalDeleteField.ObjectFieldName;
                }
                PropertyInfo property = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                this.SetLogicalDeleteFunction = delegate (T model, bool isDelete) {
                    property.SetValue(model, isDelete, null);
                    return model;
                };
            }
        }

        private void BindSetModifyDatetimeFunction()
        {
            Func<PropertyInfo, bool> predicate = null;
            this.SetModifyDatetimeFunction = null;
            if (this.UpdateDatetimeField != null)
            {
                if (predicate == null)
                {
                    predicate = t => t.Name == base.UpdateDatetimeField.ObjectFieldName;
                }
                PropertyInfo property = this.PropertyInfos.FirstOrDefault<PropertyInfo>(predicate);
                this.SetModifyDatetimeFunction = delegate (T model, DateTime time) {
                    property.SetValue(model, time, null);
                    return model;
                };
            }
        }

        private void BindUpdateCloneFunction()
        {
            this.UpdateCloneFunction = null;
            if (this.UpdateExceptFields.Count > 0)
            {
                Func<T, T, T> cloneFunction = this.GetCloneModelFunction((from t in this.UpdateExceptFields select t.ObjectFieldName).ToArray<string>());
                this.UpdateCloneFunction = delegate (IDbContext context, T model, string[] updateFields) {
                    Func<T, T, T> cloneModelFunction = cloneFunction;
                    if (updateFields != null)
                    {
                    }
                    if ((<>c__DisplayClass43<T>.CS$<>9__CachedAnonymousMethodDelegate45 == null) && updateFields.Any<string>(<>c__DisplayClass43<T>.CS$<>9__CachedAnonymousMethodDelegate45))
                    {
                        cloneModelFunction = this.GetCloneModelFunction((from t in this.AllFields
                            where this.UpdateExceptFields.Contains(t) || (!t.ReadOnly && !updateFields.Contains<string>(t.ObjectFieldName))
                            select t.ObjectFieldName).ToArray<string>());
                    }
                    DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(typeof(T)), true);
                    object obj2 = model.GetValue(pkDynamicPropertyInfo.PropertyName);
                    T local = context.QueryObject<T>(pkDynamicPropertyInfo.DataFieldName, obj2, true);
                    cloneModelFunction(model, local);
                    return local;
                };
            }
        }

        public void CheckDuplicate(IDbContext context, T model, params string[] fields)
        {
            if (this.CheckDuplicateFunction != null)
            {
                this.CheckDuplicateFunction(context, model, fields);
            }
        }

        public IRuleMapper<T> ClearAllCompositeUniqueKeyFields()
        {
            this.CompositeUniqueKeyFields.Clear();
            return this;
        }

        public IRuleMapper<T> ClearAllCreateExceptFields()
        {
            this.CreateExceptFields.Clear();
            return this;
        }

        public IRuleMapper<T> ClearAllUpdateExceptFields()
        {
            this.UpdateExceptFields.Clear();
            return this;
        }

        public IRuleMapper<T> ClearCodeField()
        {
            this.CodeField = null;
            return this;
        }

        public IRuleMapper<T> ClearCompositeUniqueKeyFields(params Expression<Func<T, object>>[] fields)
        {
            List<string> list = new List<string>();
            foreach (Expression<Func<T, object>> expression in fields)
            {
                string propertyName = this.GetPropertyName(expression);
                if (!list.Contains(propertyName))
                {
                    list.Add(propertyName);
                }
            }
            DataFieldAttribute[] item = this.CompositeUniqueKeyFields.FirstOrDefault<DataFieldAttribute[]>(t => list.All<string>(f => t.Any<DataFieldAttribute>(i => i.ObjectFieldName == f)));
            if (item != null)
            {
                this.CompositeUniqueKeyFields.Remove(item);
            }
            return this;
        }

        public IRuleMapper<T> ClearCreateDatetimeField()
        {
            this.CreateDatetimeField = null;
            return this;
        }

        public IRuleMapper<T> ClearCreateExceptField(Expression<Func<T, object>> field)
        {
            List<DataFieldAttribute> createExceptFields = this.CreateExceptFields;
            string fieldname = this.GetPropertyName(field);
            DataFieldAttribute item = createExceptFields.FirstOrDefault<DataFieldAttribute>(t => t.ObjectFieldName == fieldname);
            if (item != null)
            {
                createExceptFields.Remove(item);
            }
            return this;
        }

        public IRuleMapper<T> ClearCreaterField()
        {
            this.CreaterField = null;
            return this;
        }

        public IRuleMapper<T> ClearLogicalDeleteField()
        {
            this.LogicalDeleteField = null;
            return this;
        }

        public IRuleMapper<T> ClearModifyDatetimeField()
        {
            this.UpdateDatetimeField = null;
            return this;
        }

        public IRuleMapper<T> ClearNameField()
        {
            this.NameField = null;
            return this;
        }

        public IRuleMapper<T> ClearPrimaryKeyField()
        {
            this.PrimaryKeyField = null;
            return this;
        }

        public IRuleMapper<T> ClearUpdateExceptField(Expression<Func<T, object>> field)
        {
            List<DataFieldAttribute> updateExceptFields = this.UpdateExceptFields;
            string fieldname = this.GetPropertyName(field);
            DataFieldAttribute item = updateExceptFields.FirstOrDefault<DataFieldAttribute>(t => t.ObjectFieldName == fieldname);
            if (item != null)
            {
                updateExceptFields.Remove(item);
            }
            return this;
        }

        private void CloneDictionary(IDictionary newObj, IDictionary local)
        {
            IDictionaryEnumerator enumerator = newObj.GetEnumerator();
            while (enumerator.MoveNext())
            {
                local.Add(enumerator.Key, enumerator.Value);
            }
        }

        private void CloneList(IList newObj, IList local)
        {
            for (int i = 0; i < newObj.Count; i++)
            {
                local.Add(newObj[i]);
            }
        }

        public T CreateClone(IDbContext context, T model)
        {
            if (this.CreateCloneFunction != null)
            {
                model = this.CreateCloneFunction(context, model);
            }
            return model;
        }

        IRuleMapper<T> IRuleMapper<T>.RemoveRule<F>(Expression<Func<T, F>> field)
        {
            base.RemoveRule<F>(field);
            return this;
        }

        public string FixCondition(string condition)
        {
            condition = condition ?? "";
            if (!string.IsNullOrWhiteSpace(condition))
            {
                condition = string.Format(" ({0}) and ({1}) ", this.GetDefaultCondition(), condition);
                return condition;
            }
            condition = this.GetDefaultCondition();
            return condition;
        }

        private Func<T, bool> FixCondition<F>(Func<T, bool> conditoin, Expression<Func<T, F>> field)
        {
            string name = (field.Body as MemberExpression).Member.Name;
            return t => ((this.conditoin == null) ? true : this.conditoin(t));
        }

        private void FixSortSetting(ref string sortPropertyName, ref bool desc)
        {
            if (!string.IsNullOrWhiteSpace(sortPropertyName))
            {
                string temp = sortPropertyName;
                if (this.AllFields.All<DataFieldAttribute>(t => t.DataFieldName != temp))
                {
                    sortPropertyName = this.GetDbField(sortPropertyName, false, true).DataFieldName;
                }
            }
            else if (this.CodeField != null)
            {
                sortPropertyName = this.CodeField.DataFieldName;
                desc = false;
            }
            else if (this.NameField != null)
            {
                sortPropertyName = this.NameField.DataFieldName;
                desc = false;
            }
            else if (this.CreateDatetimeField != null)
            {
                sortPropertyName = this.CreateDatetimeField.DataFieldName;
                desc = true;
            }
            else
            {
                sortPropertyName = this.PrimaryKeyField.DataFieldName;
                desc = false;
            }
        }

        private string FormatFieldName(string fieldName)
        {
            if ((fieldName == null) || (fieldName.IndexOf("[") > 1))
            {
                return null;
            }
            return string.Format("[{0}]", fieldName);
        }

        private Func<T, T, T> GetCloneModelFunction(params string[] ignoreFields)
        {
            List<Action<T, T>> CloneFieldValueFunctionList = new List<Action<T, T>>();
            foreach (PropertyInfo info in this.PropertyInfos)
            {
                if (info.CanWrite && !ignoreFields.Contains<string>(info.Name))
                {
                    Action<T, T> action2 = null;
                    Action<T, T> action3 = null;
                    Action<T, T> action4 = null;
                    Func<T, object[], object> getValue = new Func<T, object[], object>(info.GetValue);
                    Action<T, object, object[]> setValue = new Action<T, object, object[]>(info.SetValue);
                    Action<T, T> item = null;
                    if (info.PropertyType.IsSubclassOf(typeof(IList)))
                    {
                        if (action2 == null)
                        {
                            action2 = delegate (T newObj, T local) {
                                object obj2 = getValue(newObj, null);
                                object obj3 = getValue(local, null);
                                if (obj3 == null)
                                {
                                    setValue(local, obj3, null);
                                }
                                this.CloneList(obj2 as IList, obj3 as IList);
                            };
                        }
                        item = action2;
                    }
                    else if (info.PropertyType.IsSubclassOf(typeof(IDictionary)))
                    {
                        if (action3 == null)
                        {
                            action3 = delegate (T newObj, T local) {
                                object obj2 = getValue(newObj, null);
                                object obj3 = getValue(local, null);
                                if (obj3 == null)
                                {
                                    setValue(local, obj3, null);
                                }
                                this.CloneDictionary(obj2 as IDictionary, obj3 as IDictionary);
                            };
                        }
                        item = action3;
                    }
                    else
                    {
                        if (action4 == null)
                        {
                            action4 = delegate (T newObj, T local) {
                                object objB = getValue(newObj, null);
                                if (!object.Equals(getValue(local, null), objB))
                                {
                                    setValue(local, objB, null);
                                }
                            };
                        }
                        item = action4;
                    }
                    CloneFieldValueFunctionList.Add(item);
                }
            }
            return delegate (T newModel, T localModel) {
                CloneFieldValueFunctionList.ForEach(delegate (Action<T, T> cloneField) {
                    cloneField(newModel, localModel);
                });
                return localModel;
            };
        }

        private string GetConditionByPropertyValues(params Tuple<DataFieldAttribute, string>[] propertyValues)
        {
            SqlStringBuilder builder = new SqlStringBuilder("");
            if (propertyValues != null)
            {
                foreach (Tuple<DataFieldAttribute, string> tuple in propertyValues)
                {
                    if (tuple.Item2 == null)
                    {
                        builder.AppendFormat(" AND {0} is null", this.FormatFieldName(tuple.Item1.DataFieldName));
                    }
                    else
                    {
                        builder.AppendFormat(" AND {0}= '{1}'", this.FormatFieldName(tuple.Item1.DataFieldName), tuple.Item2);
                    }
                }
            }
            return builder.ToString();
        }

        private int GetCountByPropertyValues(IDbContext context, params Tuple<DataFieldAttribute, string>[] propertyValues)
        {
            SqlStringBuilder builder = new SqlStringBuilder("");
            if (propertyValues != null)
            {
                foreach (Tuple<DataFieldAttribute, string> tuple in propertyValues)
                {
                    if (tuple.Item2 == null)
                    {
                        builder.AppendFormat(" AND {0} is null", this.FormatFieldName(tuple.Item1.DataFieldName));
                    }
                    else
                    {
                        builder.AppendFormat(" AND {0}= '{1}'", this.FormatFieldName(tuple.Item1.DataFieldName), tuple.Item2);
                    }
                }
            }
            return context.QueryCount<T>(builder.ToString());
        }

        protected DataFieldAttribute GetDbField(string propertyName, bool filterReadOnly, bool checkExsit)
        {
            DataFieldAttribute attribute = this.AllFields.FirstOrDefault<DataFieldAttribute>(t => t.ObjectFieldName == propertyName);
            if (((attribute != null) && filterReadOnly) && attribute.ReadOnly)
            {
                attribute = null;
            }
            if (checkExsit && (attribute == null))
            {
                throw new ObjectValidateException(string.Format("The property({0}) of model({1}) not exists.", propertyName, typeof(T).Name), null, null);
            }
            return attribute;
        }

        public IDbObjectInfo<T> GetDbObjectInfo()
        {
            return this;
        }

        private string GetDefaultCondition()
        {
            SqlStringBuilder builder = new SqlStringBuilder("");
            if (this.LogicalDeleteField != null)
            {
                builder.AppendFormat(" {0} = 0 ", this.FormatFieldName(this.LogicalDeleteField.DataFieldName));
            }
            else
            {
                builder.Append(" 1=1 ");
            }
            return builder.ToString();
        }

        private List<T> GetListByPropertyValues(IDbContext context, string sortPropertyName = null, bool desc = true, int pageSize = 0x7fffffff, int pageIndex = 0, params Tuple<DataFieldAttribute, string>[] propertyValues)
        {
            SqlStringBuilder builder = new SqlStringBuilder("");
            builder.Append(this.GetConditionByPropertyValues(propertyValues));
            OrderBy[] orderBy = new OrderBy[1];
            OrderBy by = new OrderBy {
                Column = sortPropertyName,
                Desc = desc
            };
            orderBy[0] = by;
            return context.QueryObjects<T>(builder.ToString(), pageSize, pageIndex, orderBy);
        }

        public IModelValidator<T> GetModelValidator()
        {
            return this;
        }

        public DateTime GetModifyDatetime(T model)
        {
            DateTime minValue = DateTime.MinValue;
            if (this.GetModifyDatetimeFunction != null)
            {
                minValue = this.GetModifyDatetimeFunction(model);
            }
            return minValue;
        }

        public F GetPrimaryKey<F>(T model)
        {
            F local = default(F);
            if (this.GetPrimaryKeyFunction != null)
            {
                local = (F) this.GetPrimaryKeyFunction(model);
            }
            return local;
        }

        private string GetPropertyName(Expression<Func<T, object>> field)
        {
            if (field == null)
            {
                return null;
            }
            MemberExpression body = field.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression expression2 = field.Body as UnaryExpression;
                if (expression2 != null)
                {
                    body = expression2.Operand as MemberExpression;
                }
            }
            if (body == null)
            {
                throw new Exception(string.Format("Error Expression:{0}!", field));
            }
            return body.Member.Name;
        }

        private Func<T, string> GetRepeatConditionFunction(params DataFieldAttribute[] uniqueDataFieldAttributes)
        {
            Func<PropertyInfo, bool> predicate = null;
            if ((uniqueDataFieldAttributes == null) || (uniqueDataFieldAttributes.Length == 0))
            {
                return null;
            }
            List<Func<T, SqlStringBuilder, SqlStringBuilder>> formatFunctions = new List<Func<T, SqlStringBuilder, SqlStringBuilder>>();
            if (this.PrimaryKeyField != null)
            {
                if (predicate == null)
                {
                    predicate = t => t.Name == base.PrimaryKeyField.ObjectFieldName;
                }
                PropertyInfo info = this.PropertyInfos.First<PropertyInfo>(predicate);
                Func<T, object[], object> getValue = new Func<T, object[], object>(info.GetValue);
                formatFunctions.Add(delegate (T m, SqlStringBuilder sb) {
                    string str = getValue(m, null) as string;
                    if (str == null)
                    {
                        return sb.AppendFormat(" and {0} is not null ", this.FormatFieldName(this.PrimaryKeyField.DataFieldName));
                    }
                    return sb.AppendFormat(" and {0} <> '{1}' ", this.FormatFieldName(this.PrimaryKeyField.DataFieldName), str);
                });
            }
            foreach (PropertyInfo info2 in this.PropertyInfos)
            {
                foreach (DataFieldAttribute attribute in uniqueDataFieldAttributes)
                {
                    if (info2.Name == attribute.ObjectFieldName)
                    {
                        Func<T, object[], object> getValue = new Func<T, object[], object>(info2.GetValue);
                        string fieldName = this.FormatFieldName(attribute.DataFieldName);
                        Func<T, SqlStringBuilder, SqlStringBuilder> item = delegate (T m, SqlStringBuilder sb) {
                            object obj2 = getValue(m, null);
                            if (obj2 != null)
                            {
                                return sb.AppendFormat(" and {0} = '{1}' ", fieldName, obj2.ToString());
                            }
                            return sb.AppendFormat(" and {0} is null ", fieldName);
                        };
                        formatFunctions.Add(item);
                    }
                }
            }
            return delegate (T m) {
                SqlStringBuilder sb = new SqlStringBuilder("");
                formatFunctions.ForEach(delegate (Func<T, SqlStringBuilder, SqlStringBuilder> addCondition) {
                    addCondition(m, sb);
                });
                return sb.ToString();
            };
        }

        public T ImportClone(IDbContext context, T model)
        {
            if (this.ImportCloneFunction != null)
            {
                model = this.ImportCloneFunction(context, model);
            }
            return model;
        }

        protected void Init()
        {
            this.CompositeUniqueKeyFields = new List<DataFieldAttribute[]>();
            this.CreateExceptFields = new List<DataFieldAttribute>();
            this.UpdateExceptFields = new List<DataFieldAttribute>();
            this.PropertyInfos = typeof(T).GetProperties();
            this.DbTable = (DataTableAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(DataTableAttribute));
            List<DataFieldAttribute> list = new List<DataFieldAttribute>();
            foreach (PropertyInfo info in this.PropertyInfos)
            {
                DataFieldAttribute dataFieldAttribute = info.GetDataFieldAttribute();
                if (dataFieldAttribute != null)
                {
                    dataFieldAttribute.ObjectFieldName = info.Name;
                    list.Add(dataFieldAttribute);
                }
            }
            this.AllFields = list.ToArray();
            this.PrimaryKeyField = this.AllFields.FirstOrDefault<DataFieldAttribute>(t => t.PrimaryKey);
            DataFieldAttribute[] collection = (from t in this.AllFields
                where t.ReadOnly
                select t).ToArray<DataFieldAttribute>();
            this.CreateExceptFields.AddRange(collection);
            this.UpdateExceptFields.AddRange(collection);
        }

        public IRuleMapper<T> PartialRuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, DbOperation dbOperation = 3)
        {
            base.AddRule<F>(field, func, this.FixCondition<F>(condition, field), dbOperation.ToActions(), true);
            return this;
        }

        public IRuleMapper<T> PartialRuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, DbOperation dbOperation = 3)
        {
            base.AddRule<F1, F2>(field, compareField, compareFunc, this.FixCondition<F1>(condition, field), dbOperation.ToActions(), true);
            return this;
        }

        public T RemoveClone(IDbContext context, T model)
        {
            if (this.RemoveCloneFunction != null)
            {
                model = this.RemoveCloneFunction(context, model);
            }
            return model;
        }

        public IRuleMapper<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, DbOperation dbOperation = 3)
        {
            base.AddRule<F>(field, func, this.FixCondition<F>(condition, field), dbOperation.ToActions(), false);
            return this;
        }

        public IRuleMapper<T> RuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, DbOperation dbOperation = 3)
        {
            base.AddRule<F1, F2>(field, compareField, compareFunc, this.FixCondition<F1>(condition, field), dbOperation.ToActions(), false);
            return this;
        }

        public IRuleMapper<T> SetCodeField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, true, true);
            this.CodeField = attribute;
            return this;
        }

        public IRuleMapper<T> SetCreateDatetimeField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, false, false);
            this.CreateDatetimeField = attribute;
            return this;
        }

        public IRuleMapper<T> SetCreaterField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, true, true);
            this.CreaterField = attribute;
            return this;
        }

        public IRuleMapper<T> SetDbLog(bool isDbLog)
        {
            this.SetDbLog(isDbLog);
            return this;
        }

        protected void SetDefault()
        {
            this.CreateDatetimeField = this.GetDbField("CreateDatetime", true, false);
            this.UpdateDatetimeField = this.GetDbField("UpdateDatetime", true, false);
            this.CreaterField = this.GetDbField("CreateUserId", true, false);
            this.LogicalDeleteField = this.GetDbField("Invalid", true, false);
            this.CodeField = this.GetDbField("Code", true, false);
            this.NameField = this.GetDbField("Name", true, false);
            if (this.PrimaryKeyField != null)
            {
                this.CreateExceptFields.Add(this.PrimaryKeyField);
                this.UpdateExceptFields.Add(this.PrimaryKeyField);
            }
            if (this.CodeField != null)
            {
                this.UpdateExceptFields.Add(this.CodeField);
                this.CompositeUniqueKeyFields.Add(new DataFieldAttribute[] { this.CodeField });
            }
            if (this.CreateDatetimeField != null)
            {
                this.CreateExceptFields.Add(this.CreateDatetimeField);
                this.UpdateExceptFields.Add(this.CreateDatetimeField);
            }
            if (this.CreaterField != null)
            {
                this.UpdateExceptFields.Add(this.CreaterField);
            }
            if (this.LogicalDeleteField != null)
            {
                this.CreateExceptFields.Add(this.LogicalDeleteField);
                this.UpdateExceptFields.Add(this.LogicalDeleteField);
            }
        }

        public T SetLogicalDelete(T model)
        {
            if (this.SetLogicalDeleteFunction != null)
            {
                this.SetLogicalDeleteFunction(model, true);
            }
            return model;
        }

        public IRuleMapper<T> SetLogicalDeleteField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, false, false);
            this.LogicalDeleteField = attribute;
            return this;
        }

        public T SetModifyDatetime(T model)
        {
            if (this.SetModifyDatetimeFunction != null)
            {
                this.SetModifyDatetimeFunction(model, DateTime.Now);
            }
            return model;
        }

        public IRuleMapper<T> SetModifyDatetimeField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, false, false);
            this.UpdateDatetimeField = attribute;
            return this;
        }

        public IRuleMapper<T> SetNameField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, true, true);
            this.NameField = attribute;
            return this;
        }

        public IRuleMapper<T> SetPrimaryKeyField(Expression<Func<T, object>> field)
        {
            string propertyName = this.GetPropertyName(field);
            DataFieldAttribute attribute = (propertyName == null) ? null : this.GetDbField(propertyName, true, true);
            this.PrimaryKeyField = attribute;
            return this;
        }

        public virtual IRuleMapper<T> StartUp()
        {
            this.Bind();
            return this;
        }

        public T UpdateClone(IDbContext context, T model, params string[] updateFields)
        {
            if (this.UpdateCloneFunction != null)
            {
                model = this.UpdateCloneFunction(context, model, updateFields);
            }
            return model;
        }

        public bool Validate(T model, DbOperation dbOperation)
        {
            return base.Validate(model, dbOperation.ToString());
        }

        public bool Validate(object model, DbOperation dbOperation = 3)
        {
            return this.Validate((T) model, dbOperation);
        }

        protected DataFieldAttribute[] AllFields { get; set; }

        private Action<IDbContext, T, string[]> CheckDuplicateFunction { get; set; }

        protected DataFieldAttribute CodeField { get; set; }

        protected List<DataFieldAttribute[]> CompositeUniqueKeyFields { get; set; }

        private Func<IDbContext, T, T> CreateCloneFunction { get; set; }

        protected DataFieldAttribute CreateDatetimeField { get; set; }

        protected List<DataFieldAttribute> CreateExceptFields { get; set; }

        protected DataFieldAttribute CreaterField { get; set; }

        protected DataTableAttribute DbTable { get; set; }

        private Func<string[], string> GetKeywordConditionFunction { get; set; }

        private Func<T, DateTime> GetModifyDatetimeFunction { get; set; }

        private Func<T, object> GetPrimaryKeyFunction { get; set; }

        private Func<IDbContext, T, T> ImportCloneFunction { get; set; }

        public bool IsDbLog { get; set; }

        public bool IsLogicalDelete
        {
            get
            {
                return (this.LogicalDeleteField != null);
            }
        }

        protected DataFieldAttribute LogicalDeleteField { get; set; }

        protected DataFieldAttribute NameField { get; set; }

        protected DataFieldAttribute PrimaryKeyField { get; set; }

        protected PropertyInfo[] PropertyInfos { get; set; }

        private Func<IDbContext, T, T> RemoveCloneFunction { get; set; }

        private Func<T, bool, T> SetLogicalDeleteFunction { get; set; }

        private Func<T, DateTime, T> SetModifyDatetimeFunction { get; set; }

        private Func<IDbContext, T, string[], T> UpdateCloneFunction { get; set; }

        protected DataFieldAttribute UpdateDatetimeField { get; set; }

        protected List<DataFieldAttribute> UpdateExceptFields { get; set; }
    }
}

