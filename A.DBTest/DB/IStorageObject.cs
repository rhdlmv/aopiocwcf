namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;

    public interface IStorageObject
    {
        object GetValue(string propertyName);
        void OnRead(object sender, DbEventArgs e);
        void OnWrote(object sender, DbEventArgs e);
        void SetValue(string propertyName, object value);

        bool IsNewObject { get; }

        Dictionary<string, ValueInfo> ModifiedValues { get; }
    }
}

