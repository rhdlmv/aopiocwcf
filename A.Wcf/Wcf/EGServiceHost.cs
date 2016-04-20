namespace AOPIOC.Wcf
{
    using System;
    using System.ServiceModel;

    public class EGServiceHost : ServiceHost
    {
        public EGServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            if (base.Description.Behaviors.Find<EGServiceBehavior>() == null)
            {
                base.Description.Behaviors.Add(new EGServiceBehavior());
            }
        }
    }
}

