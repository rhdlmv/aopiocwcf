namespace System
{
    using System.Runtime.CompilerServices;

    public static class ExceptionExtension
    {
        public static bool Include<T>(this Exception ex) where T: Exception
        {
            Exception innerException = ex;
            T local = default(T);
            while ((local == null) && (innerException != null))
            {
                local = innerException as T;
                innerException = innerException.InnerException;
            }
            return (local != null);
        }
    }
}

