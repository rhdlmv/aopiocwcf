namespace AOPIOC.Wcf
{
    using EgoalTech.UserAccessManage.Model;
    using System;
    using System.Runtime.CompilerServices;

    public class UserAccessInfo
    {
        public EgoalTech.UserAccessManage.Model.Organization Organization { get; set; }

        public string[] PermissionCodes { get; set; }

        public EgoalTech.UserAccessManage.Model.User User { get; set; }

        public EgoalTech.UserAccessManage.Model.UserAccess UserAccess { get; set; }
    }
}

