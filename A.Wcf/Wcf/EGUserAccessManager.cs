using EgoalTech.DB;
using EgoalTech.UserAccessManage;
using System;

namespace AOPIOC.Wcf
{
    public class EGUserAccessManager : UserAccessManager
    {
        public EGUserAccessManager(DbObjectOperator op, IPasswordEncryptor encryptor) : base(op)
        {
            base.AccountManager.PasswordEncryptor = encryptor;
        }
    }
}

