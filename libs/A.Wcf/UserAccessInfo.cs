namespace AOPIOC.Wcf
{
    using A.UserAccessManage.Model;
    using System;
    

    public class UserAccessInfo
    {
        public A.UserAccessManage.Model.Organization Organization { get; set; }

        public string[] PermissionCodes { get; set; }

        public A.UserAccessManage.Model.User User { get; set; }

        public A.UserAccessManage.Model.UserAccess UserAccess { get; set; }
    }
}

