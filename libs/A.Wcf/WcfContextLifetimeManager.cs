namespace AOPIOC.Wcf
{
    using Microsoft.Practices.Unity;
    using System;
    
    using System.ServiceModel;

    public class WcfContextLifetimeManager : LifetimeManager
    {
        public WcfContextLifetimeManager()
        {
            if (OperationContext.Current == null)
            {
                this.Key = Guid.NewGuid().ToString();
            }
            else
            {
                this.Key = OperationContext.Current.GetHashCode().ToString();
            }
        }

        public override object GetValue()
        {
            return WcfContext.Current.Get<object>(this.Key);
        }

        public override void RemoveValue()
        {
            WcfContext.Current.Remove(this.Key);
        }

        public override void SetValue(object newValue)
        {
            WcfContext.Current.Set(this.Key, newValue);
        }

        private string Key { get; set; }
    }
}

