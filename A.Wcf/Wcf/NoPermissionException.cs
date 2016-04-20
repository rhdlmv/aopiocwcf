using EgoalTech.Commons.Exception;
using System;

namespace AOPIOC.Wcf
{
    public class NoPermissionException : GenericException
    {
        public NoPermissionException(string message) : base(message)
        {
        }

        public NoPermissionException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "WCF00-001";
            }
        }
    }
}

