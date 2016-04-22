namespace A.WcfCommonOperation
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    [ServiceContract(Namespace="")]
    public interface ICommonOperationService
    {
        [OperationContract]
        string Create(string opKey, string objectXml);
        [OperationContract]
        string Load(string opKey, string condition, string objectType, bool checkExist = false);
        [OperationContract]
        string Query(string opKey, string condition, string objectType, int rowCount, int pageIndex, string sort);
        [OperationContract]
        int QueryCount(string opKey, string condition, string objectType);
        [OperationContract]
        void Remove(string opKey, string objectXml);
        [OperationContract]
        void RemoveById(string opKey, string id, string objectType);
        [OperationContract]
        string Update(string opKey, string objectXml);
    }
}

