namespace A.Validation
{
    using System;

    public interface IRuleContext
    {
        void Register<T>(IObjectValidator<T> validator);
        bool Validate(object model, string action);
        bool Validate<T>(T model, string action);
    }
}

