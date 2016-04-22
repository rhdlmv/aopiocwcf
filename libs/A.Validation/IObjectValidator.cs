namespace A.Validation
{
    using System;

    public interface IObjectValidator
    {
        bool Validate(object model, string action);
    }
}

