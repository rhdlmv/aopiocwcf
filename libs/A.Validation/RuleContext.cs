namespace A.Validation
{
    using System;
    using System.Collections.Generic;

    public class RuleContext : IRuleContext
    {
        private Dictionary<Type, IObjectValidator> Validators = new Dictionary<Type, IObjectValidator>();

        public void Register<T>(IObjectValidator<T> validator)
        {
            lock (this.Validators)
            {
                Type key = typeof(T);
                if (this.Validators.ContainsKey(key))
                {
                    this.Validators[key] = validator;
                }
                else
                {
                    this.Validators.Add(key, validator);
                }
            }
        }

        public bool Validate(object model, string action)
        {
            IObjectValidator validator = null;
            for (Type type = model.GetType(); (validator == null) && (type != null); type = type.BaseType)
            {
                this.Validators.TryGetValue(type, out validator);
            }
            if (validator != null)
            {
                return validator.Validate(model, action);
            }
            return true;
        }

        public bool Validate<T>(T model, string action)
        {
            return this.Validate<T>(model, action);
        }
    }
}

