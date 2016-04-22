using A.DB;

namespace A.DBExtension
{
    public interface IDbRuleContext
    {
        IDbObjectInfo<T> GetDbObjectInfo<T>() where T : DbObject, new();
        IModelValidator<T> GetValidator<T>() where T : DbObject, new();
        IDbRuleContext Register<T>(IRuleMapper<T> validator) where T : DbObject, new();
    }
}

