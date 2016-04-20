namespace AOPIOC.Wcf
{
    using System;

    public interface IObjectContainer
    {
        object GetLifecycleHandler(int lifecycle);
        bool IsRegistered(Type type);
        bool IsRegistered(Type type, string name);
        void RegisterInstance<T>(T instance);
        void RegisterInstance<T>(string name, T instance);
        void RegisterType<T>(params object[] members);
        void RegisterType<F, T>(params object[] members) where T: F;
        void RegisterType<T>(int lifecycle, params object[] members);
        void RegisterType<F, T>(int lifecycle, params object[] members) where T: F;
        void RegisterType<T>(string name, params object[] members);
        void RegisterType<F, T>(string name, params object[] members) where T: F;
        void RegisterType<T>(string name, int lifecycle, params object[] members);
        void RegisterType<F, T>(string name, int lifecycle, params object[] members) where T: F;
        void RegisterType(Type from, Type to, string name, params object[] members);
        void RegisterType(Type from, Type to, string name, int lifecycle, params object[] members);
        void RegisterTypeWithConstructor<T>(params object[] constructors);
        void RegisterTypeWithConstructor<T>(int lifecycle, params object[] constructors);
        void RegisterTypeWithConstructor<T>(string name, params object[] constructors);
        void RegisterTypeWithConstructor<T>(string name, int lifecycle, params object[] constructors);
        T Resolve<T>();
        T Resolve<T>(string name);
        object Resolve(Type type);
        object Resolve(Type type, string name);
    }
}

