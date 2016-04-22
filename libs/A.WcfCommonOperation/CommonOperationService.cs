using A.Commons.Utils;
using A.DB;
using A.UserAccessManage.Model;
using AOPIOC.Wcf;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.ServiceModel.Activation;

namespace A.WcfCommonOperation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommonOperationService : ICommonOperationService
    {
        private A.WcfCommonOperation.CommonOperation _commonOperation = null;

        [Transaction, Authentication]
        public string Create(string opKey, string objectXml)
        {
            User user = this.GetUser();
            Type objectTypeFromXml = this.GetCommonOperation().GetObjectTypeFromXml(objectXml);
            this.GetExecutionRule().CheckExecution(user, objectTypeFromXml, A.WcfCommonOperation.Action.CREATE);
            return SerializationUtils.ToXml(this.GetCommonOperation().Create(user, objectXml));
        }

        protected virtual A.WcfCommonOperation.CommonOperation GetCommonOperation()
        {
            if (this._commonOperation == null)
            {
                lock (this)
                {
                    if (this._commonOperation == null)
                    {
                        string connectionName = ConfigurationManager.AppSettings["common-operation-connection-name"];
                        this._commonOperation = new A.WcfCommonOperation.CommonOperation(connectionName);
                    }
                }
            }
            return this._commonOperation;
        }

        protected virtual IExecutionRule GetExecutionRule()
        {
            IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
            if (!objectContainer.IsRegistered(typeof(IExecutionRule)))
            {
                objectContainer.RegisterType<IExecutionRule, NullExecutionRule>(new object[0]);
            }
            return objectContainer.Resolve<IExecutionRule>();
        }

        private User GetUser()
        {
            UserAccessInfo userAccessInfo = WcfContext.Current.GetUserAccessInfo();
            if (userAccessInfo == null)
            {
                return null;
            }
            return userAccessInfo.User;
        }

        [Authentication]
        public string Load(string opKey, string condition, string objectType, bool checkExist = false)
        {
            string str = null;
            Type modelType = this.GetCommonOperation().GetObjectType(objectType, null);
            User user = this.GetUser();
            this.GetExecutionRule().CheckExecution(user, modelType, A.WcfCommonOperation.Action.READ);
            DbObject obj2 = this.GetCommonOperation().Load(condition, objectType, checkExist);
            if (obj2 != null)
            {
                str = SerializationUtils.ToXml(obj2);
            }
            return str;
        }

        [Authentication]
        public string Query(string opKey, string condition, string objectType, int rowCount, int pageIndex, string sort)
        {
            Type modelType = this.GetCommonOperation().GetObjectType(objectType, null);
            User user = this.GetUser();
            this.GetExecutionRule().CheckExecution(user, modelType, A.WcfCommonOperation.Action.READ);
            return SerializationUtils.ToXml(this.GetCommonOperation().Query(condition, objectType, rowCount, pageIndex, sort));
        }

        [Authentication]
        public int QueryCount(string opKey, string condition, string objectType)
        {
            Type modelType = this.GetCommonOperation().GetObjectType(objectType, null);
            User user = this.GetUser();
            this.GetExecutionRule().CheckExecution(user, modelType, A.WcfCommonOperation.Action.READ);
            return this.GetCommonOperation().QueryCount(condition, objectType);
        }

        [Authentication, Transaction]
        public void Remove(string opKey, string objectXml)
        {
            Type objectTypeFromXml = this.GetCommonOperation().GetObjectTypeFromXml(objectXml);
            User user = this.GetUser();
            this.GetExecutionRule().CheckExecution(user, objectTypeFromXml, A.WcfCommonOperation.Action.DELETE);
            this.GetCommonOperation().Remove(objectXml);
        }

        [Authentication, Transaction]
        public void RemoveById(string opKey, string id, string objectType)
        {
            Type modelType = this.GetCommonOperation().GetObjectType(objectType, null);
            User user = this.GetUser();
            this.GetExecutionRule().CheckExecution(user, modelType, A.WcfCommonOperation.Action.DELETE);
            this.GetCommonOperation().Remove(id, objectType);
        }

        [Authentication, Transaction]
        public string Update(string opKey, string objectXml)
        {
            User user = this.GetUser();
            Type objectTypeFromXml = this.GetCommonOperation().GetObjectTypeFromXml(objectXml);
            this.GetExecutionRule().CheckExecution(user, objectTypeFromXml, A.WcfCommonOperation.Action.UPDATE);
            return SerializationUtils.ToXml(this.GetCommonOperation().Update(user, objectXml));
        }
    }
}

