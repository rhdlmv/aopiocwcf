namespace AOPIOC.Wcf
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class EGServiceBehavior : IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            string name = serviceHostBase.Description.Name;
            Type serviceType = serviceHostBase.Description.ServiceType;
            IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher dispatcher2 in dispatcher.Endpoints)
                {
                    foreach (ServiceEndpoint endpoint in serviceHostBase.Description.Endpoints)
                    {
                        Type contractType = endpoint.Contract.ContractType;
                        if (((endpoint.Contract.Name == dispatcher2.ContractName) && (endpoint.Contract.Namespace == dispatcher2.ContractNamespace)) && (endpoint.Contract.ContractType != null))
                        {
                            if (serviceType.GetInterfaces().Any<Type>(t => (t == contractType)) && !objectContainer.IsRegistered(contractType))
                            {
                                objectContainer.RegisterType(contractType, serviceType, name, new object[0]);
                            }
                            dispatcher2.DispatchRuntime.InstanceProvider = new EGInstanceProvider(contractType, serviceType, name);
                            lock (this)
                            {
                                foreach (OperationDescription description in endpoint.Contract.Operations)
                                {
                                    if ((description.Behaviors != null) && ((from t in description.Behaviors
                                        where t.GetType() == typeof(EGOperationBehavior)
                                        select t).Count<IOperationBehavior>() <= 0))
                                    {
                                        IOperationBehavior item = new EGOperationBehavior();
                                        description.Behaviors.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}

