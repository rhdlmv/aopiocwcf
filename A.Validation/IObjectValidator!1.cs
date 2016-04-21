namespace A.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public interface IObjectValidator<T> : IObjectValidator
    {
        IObjectValidator<T> RemoveRule<F>(Expression<Func<T, F>> field);
        IObjectValidator<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, params string[] action);
        IObjectValidator<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, string, F> func, Func<T, bool> condition = null, params string[] action);
        IObjectValidator<T> RuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, params string[] action);
        bool Validate(T model, string action);

        Dictionary<string, List<FieldRule>> FieldRules { get; }
    }
}

