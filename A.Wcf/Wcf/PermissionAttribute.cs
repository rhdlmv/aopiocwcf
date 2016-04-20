using EgoalTech.Commons.Exception;
using EgoalTech.UserAccessManage;
using EgoalTech.UserAccessManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AOPIOC.Wcf
{
    public class PermissionAttribute : OperationAttribute
    {
        public PermissionAttribute(string applicationCode, params string[] operationCode)
        {
            this.ApplicationCode = applicationCode;
            this.OperationCodes = operationCode;
        }

        public override void AfterInvoke(object instance, object[] inputs, object result)
        {
        }

        public override void BeforeInvoke(object instance, object[] inputs)
        {
            this.CheckPermission();
        }

        private void CheckPermission()
        {
            UserAccessInfo userAccessInfo = WcfContext.Current.GetUserAccessInfo();
            if (userAccessInfo == null)
            {
                throw new AuthenticationException("Not login or login timeout.");
            }
            this.CheckPermission(userAccessInfo, this.ApplicationCode, this.OperationCodes);
        }

        private void CheckPermission(UserAccessInfo accessInfo, string applicationCode, string[] operationCodes)
        {
            if ((operationCodes != null) && (operationCodes.Length != 0))
            {
                if (string.IsNullOrEmpty(applicationCode))
                {
                    throw new ArgumentIsNullOrEmptyException("Missing application code.");
                }
                List<RolePolicy> userRolePolicys = WcfContext.Current.GetUserAccessManager().GetUserRolePolicys(accessInfo.User.Id, applicationCode);
                if (!(((userRolePolicys != null) && (userRolePolicys.Count != 0)) && userRolePolicys.Any<RolePolicy>(t => (operationCodes.Contains<string>(t.OperationCode) && (t.State == 0)))))
                {
                    throw new NoPermissionException("No Permission to operate.");
                }
            }
        }

        public override void OnInvokeError(object instance, object[] inputs, object result)
        {
        }

        protected string ApplicationCode { get; set; }

        protected string[] OperationCodes { get; set; }
    }
}

