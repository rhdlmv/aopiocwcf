namespace AOPIOC.Wcf
{
    using System;
    
    using System.ServiceModel.Description;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OperationAttribute : Attribute
    {
        public OperationAttribute() : this(1)
        {
        }

        public OperationAttribute(int priority)
        {
            this.Priority = priority;
        }

        public abstract void AfterInvoke(object instance, object[] inputs, object result);
        public abstract void BeforeInvoke(object instance, object[] inputs);
        public abstract void OnInvokeError(object instance, object[] inputs, object result);

        public System.ServiceModel.Description.OperationDescription OperationDescription { get; set; }

        public int Priority { get; set; }
    }
}

