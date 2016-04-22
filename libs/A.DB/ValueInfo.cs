namespace A.DB
{
    using System;
    

    public class ValueInfo
    {
        public ValueInfo()
        {
            this.Name = "";
            this.Value = null;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}

