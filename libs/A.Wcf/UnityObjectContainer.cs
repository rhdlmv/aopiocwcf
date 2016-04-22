namespace AOPIOC.Wcf
{
    using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;

    public class UnityObjectContainer : IObjectContainer
    {
        private IUnityContainer container;

        public UnityObjectContainer() : this(new UnityContainer())
        {
        }

        public UnityObjectContainer(IUnityContainer container)
        {
            this.container = container;
        }

        public virtual object GetLifecycleHandler(int lifecycle)
        {
            switch (lifecycle)
            {
                case 0:
                    return new PerResolveLifetimeManager();

                case 1:
                    return new ContainerControlledLifetimeManager();

                case 2:
                    return new PerThreadLifetimeManager();

                case 3:
                    return new WcfContextLifetimeManager();
            }
            return null;
        }

        private LifetimeManager GetLifetimeManager(int lifecycle)
        {
            return (LifetimeManager) this.GetLifecycleHandler(lifecycle);
        }

        public bool IsRegistered(Type type)
        {
            lock (this.container)
            {
                return UnityContainerExtensions.IsRegistered(this.container, type);
            }
        }

        public bool IsRegistered(Type type, string name)
        {
            lock (this.container)
            {
                return UnityContainerExtensions.IsRegistered(this.container, type, name);
            }
        }

        public void RegisterInstance<T>(T instance)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterInstance<T>(this.container, instance);
            }
        }

        public void RegisterInstance<T>(string name, T instance)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterInstance<T>(this.container, name, instance);
            }
        }

        public void RegisterType<T>(params object[] members)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<F, T>(params object[] members) where T: F
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<F, T>(this.container, this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<T>(int lifecycle, params object[] members)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<F, T>(int lifecycle, params object[] members) where T: F
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<F, T>(this.container, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<T>(string name, params object[] members)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, name, this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<F, T>(string name, params object[] members) where T: F
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<F, T>(this.container, name, this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<T>(string name, int lifecycle, params object[] members)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, name, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType<F, T>(string name, int lifecycle, params object[] members) where T: F
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<F, T>(this.container, name, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType(Type from, Type to, string name, params object[] members)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType(this.container, from, to, name, this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterType(Type from, Type to, string name, int lifecycle, params object[] members)
        {
            lock (this.container)
            {
                this.container.RegisterType(from, to, name, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionMember>(members));
            }
        }

        public void RegisterTypeWithConstructor<T>(params object[] constructors)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, this.ToObjects<InjectionConstructor>(constructors));
            }
        }

        public void RegisterTypeWithConstructor<T>(int lifecycle, params object[] constructors)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionConstructor>(constructors));
            }
        }

        public void RegisterTypeWithConstructor<T>(string name, params object[] constructors)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, name, this.ToObjects<InjectionConstructor>(constructors));
            }
        }

        public void RegisterTypeWithConstructor<T>(string name, int lifecycle, params object[] constructors)
        {
            lock (this.container)
            {
                UnityContainerExtensions.RegisterType<T>(this.container, name, this.GetLifetimeManager(lifecycle), this.ToObjects<InjectionConstructor>(constructors));
            }
        }

        public T Resolve<T>()
        {
            T instance = UnityContainerExtensions.Resolve<T>(this.container, new ResolverOverride[0]);
            return this.TryWrap<T>(instance);
        }

        public T Resolve<T>(string name)
        {
            T instance = UnityContainerExtensions.Resolve<T>(this.container, name, new ResolverOverride[0]);
            return this.TryWrap<T>(instance);
        }

        public object Resolve(Type type)
        {
            object instance = UnityContainerExtensions.Resolve(this.container, type, new ResolverOverride[0]);
            return this.TryWrap(type, instance);
        }

        public object Resolve(Type type, string name)
        {
            object instance = this.container.Resolve(type, name, new ResolverOverride[0]);
            return this.TryWrap(type, instance);
        }

        private T[] ToObjects<T>(object[] members)
        {
            List<T> list = new List<T>();
            if (members != null)
            {
                foreach (object obj2 in members)
                {
                    list.Add((T) obj2);
                }
            }
            return list.ToArray();
        }

        private T TryWrap<T>(T instance)
        {
            try
            {
                return Microsoft.Practices.EnterpriseLibrary.PolicyInjection.PolicyInjection.Wrap<T>(instance);
            }
            catch
            {
                return instance;
            }
        }

        private object TryWrap(Type type, object instance)
        {
            try
            {
                return Microsoft.Practices.EnterpriseLibrary.PolicyInjection.PolicyInjection.Wrap(type, instance);
            }
            catch
            {
                return instance;
            }
        }
    }
}

