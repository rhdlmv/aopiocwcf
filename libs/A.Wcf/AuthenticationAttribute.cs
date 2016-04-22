using A.UserAccessManage;
using A.UserAccessManage.Model;
using System;
using System.Diagnostics;

namespace AOPIOC.Wcf
{
    public class AuthenticationAttribute : OperationAttribute
    {
        public override void AfterInvoke(object instance, object[] inputs, object result)
        {
        }

        public override void BeforeInvoke(object instance, object[] inputs)
        {
            Debug.WriteLine("Authentication");
            if (!((inputs.Length != 0) && typeof(string).Equals(inputs[0].GetType())))
            {
                throw new ArgumentException("Cannot find operation key.");
            }
            this.CheckAccess(inputs[0] as string);
        }

        private void CheckAccess(string value)
        {
            UserAccessManager userAccessManager = WcfContext.Current.GetUserAccessManager();
            OperationKey operationKey = this.GetOperationKey(value);
            if (operationKey == null)
            {
                throw new ArgumentException("Invalid operation key.");
            }
            if (!userAccessManager.CheckAccess(operationKey.SessionId))
            {
                throw new AuthenticationException("Not login or login timeout.");
            }
            User user = userAccessManager.GetUser(operationKey.SessionId, true);
            if (!this.IsUserActive(user))
            {
                throw new AuthenticationException(string.Format("The user (Serial:{0}) is {1}.", user.Serial, user.Status));
            }
            Organization org = userAccessManager.GetOrganization(operationKey.SessionId);
            if (!this.IsOrganizationActive(org))
            {
                throw new AuthenticationException(string.Format("The organization (Name:{0}) is {1}.", org.Name, org.Status));
            }
            UserAccess access = userAccessManager.FindUserAccessItem(operationKey.SessionId);
            if (access == null)
            {
                throw new AuthenticationException("Cannot find login session.");
            }
            userAccessManager.KeepAlive(operationKey.SessionId);
            UserAccessInfo info = new UserAccessInfo
            {
                Organization = org,
                User = user,
                UserAccess = access
            };
            WcfContext.Current.Set("UserAccessInfo", info);
            WcfContext.Current.Set("OperationKey", operationKey);
        }

        private OperationKey GetOperationKey(string value)
        {
            IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
            if (!objectContainer.IsRegistered(typeof(IOperationKeyFormatter)))
            {
                objectContainer.RegisterType<IOperationKeyFormatter, OperationKeyFormatter>(new object[0]);
            }
            return objectContainer.Resolve<IOperationKeyFormatter>().Parse(value);
        }

        private bool IsOrganizationActive(Organization org)
        {
            if (org == null)
            {
                return false;
            }
            return "ACTIVE".ToString().Equals(org.Status);
        }

        private bool IsUserActive(User user)
        {
            if (user == null)
            {
                return false;
            }
            return "ACTIVE".ToString().Equals(user.Status);
        }

        public override void OnInvokeError(object instance, object[] inputs, object result)
        {
        }
    }
}

