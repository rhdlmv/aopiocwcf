namespace A.Validation
{
    using System;
    

    public class FieldRule
    {
        public string[] Actions { get; set; }

        public Func<object, object> GetField { get; set; }

        public Func<object, string, object> Validate { get; set; }

        public virtual Func<object, bool> When { get; set; }
    }
}

