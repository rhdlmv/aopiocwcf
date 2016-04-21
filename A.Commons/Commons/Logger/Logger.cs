namespace EgoalTech.Commons.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public abstract class Logger : ILogger
    {
        private static Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        public Logger(string name)
        {
            this.Name = name;
            this.EnableConsole = true;
            this.EnableDebug = true;
            this.EnableLog = true;
        }

        public abstract void Debug(string msg);
        public abstract void Error(Exception ex, string msg = null);
        protected string GetExceptionDetail(Exception ex)
        {
            string str = ex.Message + "\r\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                str = str + this.GetExceptionDetail(ex.InnerException);
            }
            return str;
        }

        public static ILogger GetLogger(string name, Type type)
        {
            if (loggers.ContainsKey(name))
            {
                return loggers[name];
            }
            ILogger logger = type.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { name }) as ILogger;
            if (logger != null)
            {
                loggers[name] = logger;
            }
            return logger;
        }

        public abstract void Warn(string msg);
        public abstract void Write(string msg);
        public abstract void WriteLine(string msg);

        public bool EnableConsole { get; set; }

        public bool EnableDebug { get; set; }

        public bool EnableDebugView { get; set; }

        public bool EnableLog { get; set; }

        public string Name { get; set; }
    }
}

