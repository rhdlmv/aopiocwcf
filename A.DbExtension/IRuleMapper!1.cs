namespace A.DBExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public interface IRuleMapper<T> where T: DbObject, new()
    {
        IRuleMapper<T> AddCompositeUniqueKeyFields(params Expression<Func<T, object>>[] fields);
        IRuleMapper<T> AddCreateExceptFields(params Expression<Func<T, object>>[] fields);
        IRuleMapper<T> AddUpdateExceptFields(params Expression<Func<T, object>>[] fields);
        IRuleMapper<T> ClearAllCompositeUniqueKeyFields();
        IRuleMapper<T> ClearAllCreateExceptFields();
        IRuleMapper<T> ClearAllUpdateExceptFields();
        IRuleMapper<T> ClearCodeField();
        IRuleMapper<T> ClearCompositeUniqueKeyFields(params Expression<Func<T, object>>[] fields);
        IRuleMapper<T> ClearCreateDatetimeField();
        IRuleMapper<T> ClearCreateExceptField(Expression<Func<T, object>> field);
        IRuleMapper<T> ClearCreaterField();
        IRuleMapper<T> ClearLogicalDeleteField();
        IRuleMapper<T> ClearModifyDatetimeField();
        IRuleMapper<T> ClearNameField();
        IRuleMapper<T> ClearPrimaryKeyField();
        IRuleMapper<T> ClearUpdateExceptField(Expression<Func<T, object>> field);
        IDbObjectInfo<T> GetDbObjectInfo();
        IModelValidator<T> GetModelValidator();
        IRuleMapper<T> PartialRuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, DbOperation dbOperation = 3);
        IRuleMapper<T> PartialRuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, DbOperation dbOperation = 3);
        IRuleMapper<T> RemoveRule<F>(Expression<Func<T, F>> field);
        IRuleMapper<T> RuleFor<F>(Expression<Func<T, F>> field, Func<F, F> func, Func<T, bool> condition = null, DbOperation dbOperation = 3);
        IRuleMapper<T> RuleFor<F1, F2>(Expression<Func<T, F1>> field, Expression<Func<T, F2>> compareField, Func<F1, F2, F1> compareFunc, Func<T, bool> condition = null, DbOperation dbOperation = 3);
        IRuleMapper<T> SetCodeField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetCreateDatetimeField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetCreaterField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetDbLog(bool isLog);
        IRuleMapper<T> SetLogicalDeleteField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetModifyDatetimeField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetNameField(Expression<Func<T, object>> field);
        IRuleMapper<T> SetPrimaryKeyField(Expression<Func<T, object>> field);
        IRuleMapper<T> StartUp();

        Dictionary<string, List<FieldRule>> FieldRules { get; }
    }
}

