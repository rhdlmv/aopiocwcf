namespace A.Commons.Exception
{
    using System;

    public class RemoveAssociatedRecordException : GenericException
    {
        public override string ErrorCode
        {
            get
            {
                return "E00-007";
            }
        }
    }
}

