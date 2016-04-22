namespace AOPIOC.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    public class EGServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new EGServiceHost(serviceType, baseAddresses);
        }
    }
}

