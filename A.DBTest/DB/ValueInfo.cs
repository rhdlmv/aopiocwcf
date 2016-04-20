namespace EgoalTech.DB
{
    using System;
    using System.Runtime.CompilerServices;

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

