namespace A.WcfCommonOperation
{
    using A.UserAccessManage.Model;
    using System;

    public interface IExecutionRule
    {
        void CheckExecution(User user, Type modelType, A.WcfCommonOperation.Action action);
    }
}

