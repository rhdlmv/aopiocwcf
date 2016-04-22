using A.DB;
using A.Validation;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Runtime.InteropServices;
namespace A.DBExtension
{
    public class DbRuleContext : IDbRuleContext
    {
        private Dictionary<Type, object> ruleMappers = new Dictionary<Type, object>();

        public IDbObjectInfo<T> GetDbObjectInfo<T>() where T : DbObject, new()
        {
            return this.GetRuleMapper<T>().GetDbObjectInfo();
        }

        public IRuleMapper<T> GetRuleMapper<T>() where T : DbObject, new()
        {
            object obj2 = null;
            Type key = typeof(T);
            if (!this.ruleMappers.TryGetValue(key, out obj2))
            {
                lock (this.ruleMappers)
                {
                    if (!this.ruleMappers.TryGetValue(key, out obj2))
                    {
                        RuleEngine<T> ruleMapper = new RuleEngine<T>();
                        this.Register<T>(ruleMapper);
                        obj2 = ruleMapper;
                    }
                }
            }
            return (obj2 as RuleEngine<T>);
        }

        public IModelValidator<T> GetValidator<T>() where T : DbObject, new()
        {
            return this.GetRuleMapper<T>().GetModelValidator();
        }

        private IRuleMapper<T> Init<T>(IRuleMapper<T> ruleMapper = null) where T : DbObject, new()
        {
            Func<object, object> item = null;
            Func<object, object> func4 = null;
            Func<object, object> func5 = null;
            Func<object, bool> func6 = null;
            ruleMapper = ruleMapper ?? new RuleEngine<T>();
            Type type = typeof(T);
            foreach (PropertyInfo info1 in type.GetProperties())
            {
                Func<object, object> func = null;
                Func<object, object> func2 = null;
                List<Func<object, object>> funcs;
                MethodInfo getMethod;
                string fieldName;
                DynamicPropertyInfo info = DbObjectTools.GetDynamicPropertyInfo(type, info1);
                if (info != null)
                {
                    Type propertyType = info.PropertyType;
                    funcs = new List<Func<object, object>>();
                    if (!info.AllowDBNull)
                    {
                        if (item == null)
                        {
                            item = o => ValidateExtend.NotNull<object>(o);
                        }
                        funcs.Add(item);
                    }
                    if ((propertyType == typeof(string)) && (info.Length > 0))
                    {
                        if (func == null)
                        {
                            func = delegate (object o)
                            {
                                if (o != null)
                                {
                                    ValidateExtend.LengthBelowOrEqual((string)o, info.Length, false);
                                }
                                return o;
                            };
                        }
                        funcs.Add(func);
                    }
                    if (((propertyType != typeof(DateTime)) && (propertyType != typeof(DateTime?))) && ((info.NumericPercision != -1) || (info.NumericScale != -1)))
                    {
                        if (func2 == null)
                        {
                            func2 = delegate (object o)
                            {
                                if (o != null)
                                {
                                    ValidateExtend.IsDecimal<object>(o, info.NumericPercision, info.NumericScale);
                                }
                                return o;
                            };
                        }
                        funcs.Add(func2);
                    }
                    if (propertyType == typeof(DateTime))
                    {
                        if (func4 == null)
                        {
                            func4 = delegate (object o)
                            {
                                if (o != null)
                                {
                                    ValidateExtend.GreaterThanOrEqual<DateTime>((DateTime)o, SqlDateTime.MinValue.Value);
                                }
                                return o;
                            };
                        }
                        funcs.Add(func4);
                    }
                    if (propertyType == typeof(DateTime?))
                    {
                        if (func5 == null)
                        {
                            func5 = delegate (object o)
                            {
                                if (o != null)
                                {
                                    DateTime? nullable = (DateTime?)o;
                                    ValidateExtend.GreaterThanOrEqual<DateTime>(nullable.Value, SqlDateTime.MinValue.Value);
                                }
                                return o;
                            };
                        }
                        funcs.Add(func5);
                    }
                    List<FieldRule> list = null;
                    if (!ruleMapper.FieldRules.TryGetValue(info1.Name, out list))
                    {
                        list = new List<FieldRule>();
                        ruleMapper.FieldRules.Add(info1.Name, list);
                    }
                    getMethod = info1.GetGetMethod();
                    fieldName = info1.Name;
                    FieldRule rule2 = new FieldRule();
                    rule2.GetField = (t => getMethod.Invoke(t, null));
                    rule2.Validate = (delegate (object o, string action)
                    {
                        Action<Func<object, object>> action2 = null;
                        object obj2;
                        try
                        {
                            if (action2 == null)
                            {
                                action2 = delegate (Func<object, object> f)
                                {
                                    f(o);
                                };
                            }
                            funcs.ForEach(action2);
                            obj2 = o;
                        }
                        catch (ValidateException exception)
                        {
                            throw new FieldValidateException(exception.Message, fieldName, exception.FieldValue);
                        }
                        return obj2;
                    });
                    rule2.Actions = (new string[] { DbOperation.Create.ToString(), (DbOperation.None | DbOperation.Update).ToString() });
                    if (func6 == null)
                    {
                        func6 = t => true;
                    }
                    rule2.When = (func6);
                    FieldRule rule = rule2;
                    list.Insert(0, rule);
                }
            }
            return ruleMapper;
        }

        public IDbRuleContext Register<T>(IRuleMapper<T> ruleMapper) where T : DbObject, new()
        {
            Type key = typeof(T);
            lock (this.ruleMappers)
            {
                ruleMapper = this.Init<T>(ruleMapper);
                if (this.ruleMappers.ContainsKey(key))
                {
                    this.ruleMappers[key] = ruleMapper;
                }
                else
                {
                    this.ruleMappers.Add(key, ruleMapper);
                }
            }
            return this;
        }
    }
}

