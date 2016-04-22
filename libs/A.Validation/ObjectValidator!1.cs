namespace A.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public class ObjectValidator<T> : IObjectValidator<T>, IObjectValidator
    {
        private Dictionary<string, List<FieldRule>> fieldRules;

        public ObjectValidator()
        {
            this.fieldRules = new Dictionary<string, List<FieldRule>>();
        }

        protected IObjectValidator<T> AddRule<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition, string[] actions, bool isPartial)
        {
            return this.AddRule<F>(field, (Func<F, string, F>)((o, act) => func(o)), condition, actions, isPartial);
        }

        protected IObjectValidator<T> AddRule<F>(Expression<Func<T, F>> field, Func<F, string, F> func, Func<T, bool> condition, string[] actions, bool isPartial)
        {
            Func<object, bool> func2 = null;
            lock (this.fieldRules)
            {
                MemberExpression body = field.Body as MemberExpression;
                string name = field.Parameters[0].Type.Name;
                string fieldname = body.Member.Name;
                Func<T, object> getFieldFunc = Expression.Lambda<Func<T, object>>(Expression.Convert(field.Body, typeof(object)), field.Parameters).Compile();
                FieldRule item = new FieldRule
                {
                    GetField = m => getFieldFunc((T)m),
                    Actions = actions,
                    Validate = delegate (object f, string action)
                    {
                        object obj2;
                        try
                        {
                            obj2 = func((F)f, action);
                        }
                        catch (ValidateException exception)
                        {
                            throw new FieldValidateException(exception.Message, fieldname, exception.FieldValue);
                        }
                        return obj2;
                    }
                };
                if (condition != null)
                {
                    if (func2 == null)
                    {
                        func2 = m => condition((T)m);
                    }
                    item.When = func2;
                }
                if (this.fieldRules.ContainsKey(fieldname))
                {
                    if (!isPartial)
                    {
                        throw new Exception(string.Format("重複添加 {0} 的驗證規則！", fieldname));
                    }
                }
                else
                {
                    this.fieldRules.Add(fieldname, new List<FieldRule>());
                }
                this.fieldRules[fieldname].Add(item);
            }
            return this;
        }

        protected IObjectValidator<T> AddRule<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition, string[] actions, bool isPartial)
        {
            Func<object, bool> func = null;
            lock (this.fieldRules)
            {
                string name = field.Parameters[0].Type.Name;
                string fieldname = (field.Body as MemberExpression).Member.Name;
                Func<T, object> getFieldFunc = Expression.Lambda<Func<T, object>>(Expression.Convert(field.Body, typeof(object)), field.Parameters).Compile();
                string str2 = (compareField.Body as MemberExpression).Member.Name;
                Func<T, object> getCompareFieldFunc = Expression.Lambda<Func<T, object>>(Expression.Convert(compareField.Body, typeof(object)), compareField.Parameters).Compile();
                FieldRule item = new FieldRule
                {
                    GetField = delegate (object m)
                    {
                        T arg = (T)m;
                        return new object[] { getFieldFunc(arg), getCompareFieldFunc(arg) };
                    },
                    Actions = actions,
                    Validate = delegate (object f, string action)
                    {
                        object obj2;
                        try
                        {
                            object[] objArray = f as object[];
                            compareFunc((F1)objArray[0], (F2)objArray[1]);
                            obj2 = objArray[0];
                        }
                        catch (ValidateException exception)
                        {
                            throw new FieldValidateException(exception.Message, fieldname, exception.FieldValue);
                        }
                        return obj2;
                    }
                };
                if (condition != null)
                {
                    if (func == null)
                    {
                        func = m => condition((T)m);
                    }
                    item.When = func;
                }
                if (this.fieldRules.ContainsKey(fieldname))
                {
                    if (!isPartial)
                    {
                        throw new Exception(string.Format("重複添加 {0} 的驗證規則！", fieldname));
                    }
                }
                else
                {
                    this.fieldRules.Add(fieldname, new List<FieldRule>());
                }
                this.fieldRules[fieldname].Add(item);
            }
            return this;
        }

        public IObjectValidator<T> RemoveRule<F>(Expression<Func<T, F>> field)
        {
            lock (this.fieldRules)
            {
                MemberExpression body = field.Body as MemberExpression;
                string name = field.Parameters[0].Type.Name;
                string str2 = body.Member.Name;
                this.fieldRules.Remove(body.Member.Name);
            }
            return this;
        }

        public IObjectValidator<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, params string[] action)
        {
            return this.AddRule<F>(field, func, condition, action, true);
        }

        public IObjectValidator<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, string, F> func, Func<T, bool> condition = null, params string[] action)
        {
            return this.AddRule<F>(field, func, condition, action, true);
        }

        public IObjectValidator<T> RuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, params string[] action)
        {
            return this.AddRule<F1, F2>(field, compareField, compareFunc, condition, action, true);
        }

        public bool Validate(T model, string action)
        {
            List<FieldValidateException> errors = new List<FieldValidateException>();
            foreach (string str in this.fieldRules.Keys)
            {
                List<FieldRule> list2 = this.fieldRules[str];
                foreach (FieldRule rule in list2)
                {
                    if (rule.Actions.Contains<string>(action) && !((rule.When == null) ? false : !rule.When(model)))
                    {
                        object obj2 = rule.GetField(model);
                        try
                        {
                            rule.Validate(obj2, action);
                        }
                        catch (FieldValidateException exception)
                        {
                            errors.Add(exception);
                        }
                    }
                }
            }
            if (errors.Count > 0)
            {
                string name = typeof(T).Name;
                throw new ObjectValidateException(string.Format("{0} validation failed！", name), name, errors);
            }
            return true;
        }

        public bool Validate(object model, string action)
        {
            return this.Validate((T)model, action);
        }

        public Dictionary<string, List<FieldRule>> FieldRules
        {
            get
            {
                return this.fieldRules;
            }
        }
    }
}

