namespace A.Commons
{
    using System;

    public class Singleton<T> where T: class, new()
    {
        private static T instance;
        private static object lockObj;

        static Singleton()
        {
            Singleton<T>.instance = default(T);
            Singleton<T>.lockObj = new object();
        }

        public static T Value
        {
            get
            {
                if (Singleton<T>.instance == null)
                {
                    lock (Singleton<T>.lockObj)
                    {
                        if (Singleton<T>.instance == null)
                        {
                            Singleton<T>.instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return Singleton<T>.instance;
            }
        }
    }
}

