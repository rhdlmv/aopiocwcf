namespace A.WcfCommonOperation
{
    using A.UserAccessManage.Model;
    using System;

    public class NullExecutionRule : IExecutionRule
    {
        public void CheckExecution(User user, Type modelType, A.WcfCommonOperation.Action action)
        {
        }
    }
}

