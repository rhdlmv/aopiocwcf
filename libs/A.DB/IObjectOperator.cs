namespace A.DB
{
    using System;

    public interface IObjectOperator
    {
        void Delete(IStorageObject obj);
        void Insert(IStorageObject obj);
        T Retrieve<T>(string condition) where T: IStorageObject, new();
        bool Retrieve(IStorageObject obj, string condition);
        T Retrieve<T>(string key_name, object key_value) where T: IStorageObject, new();
        bool Retrieve(IStorageObject obj, string key_name, object key_value);
        T RetrieveObjectAsParamter<T>() where T: IStorageObject, new();
        void StoreObjectAsParameter(IStorageObject obj);
        void Update(IStorageObject obj);
    }
}

