namespace AOPIOC.Wcf
{
    using System;
    
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public class EGInstanceProvider : IInstanceProvider
    {
        public EGInstanceProvider(Type contractType, Type serviceType, string name)
        {
            this.ContractType = contractType;
            this.ServiceType = serviceType;
            this.Name = name;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
            if (objectContainer.IsRegistered(this.ContractType, this.Name))
            {
                return objectContainer.Resolve(this.ContractType, this.Name);
            }
            return Activator.CreateInstance(this.ServiceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public Type ContractType { get; private set; }

        public string Name { get; private set; }

        public Type ServiceType { get; private set; }
    }
}

