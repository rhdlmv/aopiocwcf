namespace A.Validation
{
    using System;
    using System.Runtime.CompilerServices;

    public class FieldRule
    {
        public string[] Actions { get; set; }

        public Func<object, object> GetField { get; set; }

        public Func<object, string, object> Validate { get; set; }

        public virtual Func<object, bool> When { get; set; }
    }
}

